from docx import Document
from docx.shared import Pt
from docx.enum.text import WD_PARAGRAPH_ALIGNMENT

# Создаёт reference.docx с базовыми стилями, полезно для pandoc --reference-doc

doc = Document()
styles = doc.styles
normal = styles['Normal']
font = normal.font
font.name = 'Times New Roman'
font.size = Pt(14)

# Настроим Heading 1
h1 = styles['Heading 1']
h1.font.name = 'Times New Roman'
h1.font.size = Pt(16)
h1.font.bold = True
h1.paragraph_format.alignment = WD_PARAGRAPH_ALIGNMENT.CENTER

# Heading 2
h2 = styles['Heading 2']
h2.font.name = 'Times New Roman'
h2.font.size = Pt(14)
h2.font.bold = True
h2.paragraph_format.alignment = WD_PARAGRAPH_ALIGNMENT.LEFT

# Heading 3
h3 = styles['Heading 3']
h3.font.name = 'Times New Roman'
h3.font.size = Pt(14)
h3.font.bold = True
h3.paragraph_format.alignment = WD_PARAGRAPH_ALIGNMENT.LEFT

# Добавим пустую страницу с указанием, что это reference.docx
p = doc.add_paragraph('Reference document for pandoc. Styles set to Times New Roman, 14pt.\nUse this file with pandoc option: --reference-doc=reference.docx')
p.style = 'Normal'

doc.save('docs/reference.docx')
print('docs/reference.docx created')
