# HelpyCLI
Проект консольного помощника Helpy (на основе ИИ Phi-3 Mini).

![image](https://github.com/user-attachments/assets/608e3af6-4100-4997-a174-d434508c9e21)

# Установка
1. Скачать модель Phi-3 Mini, например, Phi-3-mini-4k-instruct-onnx (https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx);
2. Установить проект HelpyCLI на свой ПК;
3. Изменить константу ModelPath в классе Program, указав путь к установленной модели (your_path\Phi-3-mini-4k-instruct-onnx\cpu_and_mobile\cpu-int4-rtn-block-32)
4. Поменять параметры запуска и сборки проекта. Например, как это сделать в Rider:

![image](https://github.com/user-attachments/assets/aa500ef4-4e54-4ea1-a69d-549f03faf742)

5. Выходную папку (в моём случае со скриншота Tools) необходимо добавить в переменные окружения. Для этого:
6. Win + R -> выполнить команду control sysdm.cpl -> Перейти на вкладку Дополнительно -> Кнопка Переменные среды -> Выбрать переменную Path и нажать Изменить -> Кнопка Создать -> Добавить выходную папку. Результат:

![image](https://github.com/user-attachments/assets/f230eff1-30ff-4eee-b848-89abdd15c29b)
