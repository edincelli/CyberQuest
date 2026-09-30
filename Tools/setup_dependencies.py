"""Apply the documented, hash-pinned local dependency customizations (Python 3.10+)."""
from __future__ import annotations

import argparse
from collections import Counter
from dataclasses import dataclass
import hashlib
import json
from pathlib import Path
import sys


class SetupError(Exception):
    """An incompatible installation or unsafe patch was detected."""


def digest(text: str) -> str:
    return hashlib.sha256(text.encode('utf-8')).hexdigest()


def read_text(path: Path) -> str:
    return path.read_bytes().decode('utf-8-sig').replace('\r\n', '\n')


def local_path(root: Path, relative: str) -> Path:
    path = root / relative
    if Path(relative).is_absolute() or '..' in Path(relative).parts:
        raise SetupError(f'Unsafe manifest path: {relative}')
    if not path.resolve().is_relative_to(root.resolve()):
        raise SetupError(f'Path escapes project: {relative}')
    if any(part.is_symlink() or getattr(part, 'is_junction', lambda: False)()
           for part in (path, *path.parents) if part != root.parent):
        raise SetupError(f'Linked path is unsupported: {relative}')
    return path


def patched_text(text: str, edits: list[dict]) -> str:
    """Ranges are one-based lines, zero-based columns, end-exclusive, in input text."""
    lines = text.splitlines(keepends=True)
    offsets = [0]
    for line in lines:
        offsets.append(offsets[-1] + len(line))

    def position(point):
        line, column = point
        if not 1 <= line <= len(offsets) or column < 0:
            raise SetupError('Invalid manifest position')
        limit = len(lines[line - 1]) if line <= len(lines) else 0
        if column > limit:
            raise SetupError('Manifest column exceeds line')
        return offsets[line - 1] + column

    ranges = [(position(e['start']), position(e['end']), e['text']) for e in edits]
    previous = 0
    for start, end, _ in ranges:
        if start < previous or end < start:
            raise SetupError('Overlapping or unordered manifest ranges')
        previous = end
    for start, end, replacement in reversed(ranges):
        text = text[:start] + replacement + text[end:]
    return text


@dataclass
class Change:
    package: str
    path: Path
    before: bytes | None
    after: bytes


def preflight(root: Path, manifest: dict, payload_root: Path):
    changes, states, errors = [], {}, []
    seen = set()
    for entry in manifest['files']:
        package = entry['package']
        states.setdefault(package, Counter())
        try:
            path = local_path(root, entry['path'])
            if str(path).casefold() in seen:
                raise SetupError('Duplicate manifest target')
            seen.add(str(path).casefold())
            before = path.read_bytes() if path.exists() else None
            text = before.decode('utf-8-sig').replace('\r\n', '\n') if before is not None else None
            if 'payload' in entry:
                output = read_text(local_path(payload_root, entry['payload']))
                if digest(output) != entry['after_sha256']:
                    raise SetupError('Owner-authored payload hash mismatch')
                if text is not None and digest(text) != entry['after_sha256']:
                    raise SetupError('Existing addition differs; preserve it and resolve manually')
            else:
                if text is None:
                    raise SetupError('Required package file is missing')
                if digest(text) == entry['after_sha256']:
                    states[package]['already applied'] += 1
                    continue
                if digest(text) != entry['before_sha256']:
                    raise SetupError('Unsupported file hash; install the documented baseline')
                output = patched_text(text, entry['edits'])
                if digest(output) != entry['after_sha256']:
                    raise SetupError('Patch output hash mismatch')
            if text == output:
                states[package]['already applied'] += 1
            else:
                # Preserve the installed file's BOM and newline convention.
                encoded = output
                if before is not None and b'\r\n' in before:
                    encoded = encoded.replace('\n', '\r\n')
                data = encoded.encode('utf-8')
                if before is not None and before.startswith(b'\xef\xbb\xbf'):
                    data = b'\xef\xbb\xbf' + data
                changes.append(Change(package, path, before, data))
                states[package]['pending'] += 1
        except (SetupError, OSError, UnicodeError) as error:
            states[package]['incompatible'] += 1
            errors.append(f'{package}: {entry["path"]}: {error}')
    for package, counts in states.items():
        print(f'{package}: ' + ', '.join(f'{value} {key}' for key, value in counts.items()))
    if errors:
        raise SetupError('\n'.join(errors) + '\nNo files were written.')
    return changes


def apply(changes: list[Change]):
    # Recheck the complete plan before the first write, including addition conflicts.
    for change in changes:
        current = change.path.read_bytes() if change.path.exists() else None
        if current != change.before:
            raise SetupError(f'File changed after preflight: {change.path}')
    attempted = []
    try:
        for change in changes:
            change.path.parent.mkdir(parents=True, exist_ok=True)
            attempted.append(change)
            change.path.write_bytes(change.after)
    except OSError:
        for change in reversed(attempted):
            if change.before is None:
                change.path.unlink(missing_ok=True)
            else:
                change.path.write_bytes(change.before)
        raise


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--project', type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument('--check', action='store_true', help='Validate all inputs without writing any files')
    args = parser.parse_args(argv)
    try:
        if not (args.project / 'Assets').is_dir():
            raise SetupError('Project must contain an Assets directory')
        folder = Path(__file__).resolve().parent
        manifest = json.loads((folder / 'dependency_patches.json').read_text(encoding='utf-8'))
        if manifest['schema_version'] != 1:
            raise SetupError('Unsupported manifest schema')
        changes = preflight(args.project.resolve(), manifest, folder / 'OwnerAdditions')
        if not args.check:
            apply(changes)
        print(f'{"Check passed; pending" if args.check else "Applied"}: {len(changes)} file(s).')
        return 0
    except (SetupError, OSError, UnicodeError, ValueError, KeyError) as error:
        print(f'Setup failed: {error}', file=sys.stderr)
        return 1


if __name__ == '__main__':
    sys.exit(main())
