# GithubRemover

A utility for deleting GitHub repositories via GUI The WPF Interface.

## Features

- View a list of your repositories
- Delete repositories with a single click
- Support for changing languages EN-RU

## Requirements

- [.NET 8.0](https://dotnet.microsoft.com/download) (if not running from an exe)
- [GitHub CLI](https://cli.github.com/) (`gh`) - must be installed and authorized

## Installation

### Ready exe (portable)
Download [GithubRemover](https://github.com/PerfLite/Github-Remover-Repository/releases/download/v.1/GithubRemover.exe)

## Usage

1. Make sure GitHub CLI is installed: `gh --version`
2. Log in: `gh auth login`
3. Run `GithubRemover.exe`
4. Select a repository from the list
5. Click "Delete"

## How it works

The application uses the GitHub CLI (`gh repo list` and `gh repo delete`) to work with repositories. No tokens are stored - everything is done through `gh`.

## License

MIT

# GithubRemover

Утилита для удаления GitHub-репозиториев через GUI wpf Интерфейс.

## Возможности

- Просмотр списка ваших репозиториев
- Удаление репозиториев одним кликом
- Поддежка смены языков EN-RU

## Требования

- [.NET 8.0](https://dotnet.microsoft.com/download) (если запускать не из exe)
- [GitHub CLI](https://cli.github.com/) (`gh`) - должен быть установлен и авторизован

## Установка

### Готовый exe (portable)
Скачайте [GithubRemover](https://github.com/PerfLite/Github-Remover-Repository/releases/download/v.1/GithubRemover.exe)

## Использование

1. Убедитесь что GitHub CLI установлен: `gh --version`
2. Авторизуйтесь: `gh auth login`
3. Запустите `GithubRemover.exe`
4. Выберите репозиторий из списка
5. Нажмите "Удалить"

## Как это работает

Приложение использует GitHub CLI (`gh repo list` и `gh repo delete`) для работы с репозиториями. Никакие токены не хранятся - всё делается через `gh`.
