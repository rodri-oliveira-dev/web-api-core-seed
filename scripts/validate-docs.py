#!/usr/bin/env python3
"""Validate maintained documentation references and bilingual section parity."""

from __future__ import annotations

import re
import sys
from pathlib import Path
from urllib.parse import unquote, urlsplit

ROOT = Path(__file__).resolve().parents[1]
DOCUMENTS = (
    "README.md",
    "README.pt-BR.md",
    "CONTRIBUTING.md",
    "SECURITY.md",
    "CHANGELOG.md",
    "CODE_OF_CONDUCT.md",
    "docs/architecture.md",
    "docs/architecture.pt-BR.md",
    "docs/development/ef-core-migrations.md",
    "docs/development/ef-core-migrations.pt-BR.md",
    "docs/migration-from-legacy.md",
    "docs/migration-from-legacy.pt-BR.md",
    "docs/template-distribution.md",
    "docs/template-distribution.pt-BR.md",
    "docs/releases/v2.0.0.md",
    "docs/releases/release-checklist.md",
    "docs/releases/release-checklist.pt-BR.md",
    "docs/adr/README.md",
    "docs/adr/0001-explicit-ports-and-modules.md",
    "docs/adr/0002-separate-dbcontexts-and-explicit-seed.md",
    "docs/adr/0003-response-cache-and-observability.md",
)
LANGUAGE_PAIRS = (
    ("README.md", "README.pt-BR.md"),
    ("docs/releases/release-checklist.md", "docs/releases/release-checklist.pt-BR.md"),
    ("docs/template-distribution.md", "docs/template-distribution.pt-BR.md"),
    ("docs/migration-from-legacy.md", "docs/migration-from-legacy.pt-BR.md"),
    ("docs/architecture.md", "docs/architecture.pt-BR.md"),
    ("docs/development/ef-core-migrations.md", "docs/development/ef-core-migrations.pt-BR.md"),
)
LINK_PATTERN = re.compile(r"(?<!!)\[[^\]]+\]\(([^)\s]+)(?:\s+\"[^\"]*\")?\)")
HEADING_PATTERN = re.compile(r"^(#{1,6})\s+(.+?)\s*#*\s*$", re.MULTILINE)


def document_text(relative: str) -> str:
    return (ROOT / relative).read_text(encoding="utf-8-sig")


def headings(text: str) -> list[tuple[int, str]]:
    return [(len(match.group(1)), match.group(2)) for match in HEADING_PATTERN.finditer(text)]


def slug(value: str) -> str:
    value = re.sub(r"<[^>]*>", "", value).replace("`", "").lower().strip()
    value = "".join(character for character in value if character.isalnum() or character in "_- ")
    return re.sub(r"\s+", "-", value)


def validate_document(relative: str, errors: list[str]) -> int:
    source = ROOT / relative
    if not source.is_file():
        errors.append(f"{relative}: missing document")
        return 0

    body = document_text(relative)
    body = re.sub(r"^```[\s\S]*?^```\s*$", "", body, flags=re.MULTILINE)
    found = 0
    for match in LINK_PATTERN.finditer(body):
        target = match.group(1)
        if target.startswith(("https://", "http://", "mailto:", "tel:", "//")):
            continue
        url = urlsplit(unquote(target))
        if url.scheme or url.netloc:
            continue
        link_path = (source.parent / url.path).resolve() if url.path else source.resolve()
        if not link_path.is_relative_to(ROOT):
            errors.append(f"{relative}: link outside repository: {target}")
            continue
        if not link_path.exists():
            errors.append(f"{relative}: missing link target: {target}")
            continue
        if url.fragment and link_path.is_file() and link_path.suffix.lower() == ".md":
            candidates = {slug(title) for _, title in headings(link_path.read_text(encoding="utf-8-sig"))}
            if unquote(url.fragment).lower() not in candidates:
                errors.append(f"{relative}: unresolved heading in link: {target}")
                continue
        found += 1
    return found


def main() -> int:
    errors: list[str] = []
    total = sum(validate_document(path, errors) for path in DOCUMENTS)
    for english, portuguese in LANGUAGE_PAIRS:
        first = [level for level, _ in headings(document_text(english))]
        second = [level for level, _ in headings(document_text(portuguese))]
        if first != second:
            errors.append(f"{english} and {portuguese}: heading hierarchy differs")

    for error in errors:
        print(f"ERROR: {error}", file=sys.stderr)
    if errors:
        print(f"Documentation validation failed: {len(errors)} issues", file=sys.stderr)
        return 1
    print(f"Documentation validation passed: {len(DOCUMENTS)} files, {total} local links, "
          f"{len(LANGUAGE_PAIRS)} bilingual heading pairs")
    return 0


if __name__ == "__main__":
    sys.exit(main())
