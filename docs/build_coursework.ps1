# PowerShell script to create reference.docx and build coursework.docx via pandoc
# Usage: .\build_coursework.ps1

# 1) create reference.docx (requires python and python-docx)
python .\docs\make_reference_doc.py

# 2) run pandoc
$md = ".\docs\coursework.md"
$out = ".\docs\coursework.docx"
$ref = ".\docs\reference.docx"

if (Test-Path $ref) {
    pandoc $md -o $out --reference-doc=$ref --toc --toc-depth=2
} else {
    pandoc $md -o $out --toc --toc-depth=2
}

Write-Host "Generated $out"
