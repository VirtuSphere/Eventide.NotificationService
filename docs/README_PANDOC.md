Инструкция по генерации курсовой работы через pandoc

Требования на машине разработчика:
- Установлен pandoc (https://pandoc.org/installing.html)
- Установлен Python 3 и пакет `python-docx` (pip install python-docx)

Команды для генерации (PowerShell):

```powershell
cd <repo-root>
# создаёт reference.docx с нужными стилями
python .\docs\make_reference_doc.py
# генерирует coursework.docx
.\docs\build_coursework.ps1
```

Если нужно настроить поля и отступы согласно ГОСТ, откройте `reference.docx` в MS Word и настройте:
- Шрифты Normal: Times New Roman, 14pt
- Heading 1: 16pt bold, капсом и центр
- Поля страницы: Верх 2.5 см; Низ 2 см; Лево 3.0 см; Право 1.5 см

После этого повторите запуск `build_coursework.ps1`.

Примечание: pandoc генерирует .docx, но точное соответствие ГОСТ достигается через `reference.docx` — отредактируйте его вручную, сохранив корректные стили.
