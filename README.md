# GithubRemover

A utility for deleting GitHub repositories via GUI Interface.

## Features

- View a list of your repositories
- Delete repositories with a single click
- Support for changing languages EN-RU
- Modern Avalonia UI design

## Requirements

- [GitHub CLI](https://cli.github.com/) (`gh`) - must be installed and authorized

## Installation

### Ready exe (portable)
Download from [Releases](https://github.com/PerfLite/Github-Remover-Repository/releases/download/latest/GithubRemover.exe)

## Usage

1. Make sure GitHub CLI is installed: `gh --version`
2. Log in: `gh auth login`
3. Run `GithubRemover.exe`
4. Select a repository from the list
5. Click "Delete"

## How it works

The application uses the GitHub CLI (`gh repo list` and `gh repo delete`) to work with repositories. No tokens are stored - everything is done through `gh`.

## Build from source

```bash
dotnet build
```

For portable version:
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## License

MIT

---

# GithubRemover

Утилита для удаления GitHub-репозиториев через GUI Интерфейс.

## Возможности

- Просмотр списка ваших репозиториев
- Удаление репозиториев одним кликом
- Поддержка смены языков EN-RU
- Современный дизайн на Avalonia UI

## Требования

- [GitHub CLI](https://cli.github.com/) (`gh`) - должен быть установлен и авторизован

## Установка

### Готовый exe (portable)
Скачайте из [Releases](https://github.com/PerfLite/Github-Remover-Repository/releases/download/latest/GithubRemover.exe)

## Использование

1. Убедитесь что GitHub CLI установлен: `gh --version`
2. Авторизуйтесь: `gh auth login`
3. Запустите `GithubRemover.exe`
4. Выберите репозиторий из списка
5. Нажмите "Удалить"

## Как это работает

Приложение использует GitHub CLI (`gh repo list` и `gh repo delete`) для работы с репозиториями. Никакие токены не хранятся - всё делается через `gh`.

## Сборка из исходников

```bash
dotnet build
```

Для портативной версии:
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Лицензия

MIT
