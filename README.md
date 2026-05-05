# GithubRemover

Утилита для удаления GitHub-репозиториев через GUI.

## Возможности

- Просмотр списка ваших репозиториев
- Удаление репозиториев одним кликом
- Тёмная тема в стиле GitHub
- Кастомное окно подтверждения удаления

## Требования

- [.NET 8.0](https://dotnet.microsoft.com/download) (если запускать не из exe)
- [GitHub CLI](https://cli.github.com/) (`gh`) - должен быть установлен и авторизован

## Установка

### Готовый exe (portable)
Скачайте `GithubRemover` по [прямой ссылке](https://github.com/PerfLite/Github-Remover-Repository/releases/download/v.1/GithubRemover.exe)

### Из исходников
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish
```

## Использование

1. Убедитесь что GitHub CLI установлен: `gh --version`
2. Авторизуйтесь: `gh auth login`
3. Запустите `GithubRemover.exe`
4. Выберите репозиторий из списка
5. Нажмите "Удалить"

## Как это работает

Приложение использует GitHub CLI (`gh repo list` и `gh repo delete`) для работы с репозиториями. Никакие токены не хранятся - всё делается через `gh`.

## Скриншот

![Screenshot](screenshot.png)

## Лицензия

MIT
