# Todo CLI

A simple file-based task tracker CLI built with C# and .NET 10.

## Tech Stack

* C# / .NET 10
* System.CommandLine
* Spectre.Console
* JSON file storage
* xUnit

## Quick Start

```bash
git clone https://github.com/qingfa-dev/todo.cli.git
cd todo.cli
dotnet run --file task-cli.cs -- --help
```

Tasks are stored in `tasks.json` in the current directory.

## Commands

```bash
todo add "Buy groceries"
todo list
todo list done
todo list in-progress
todo update 1 "Buy milk"
todo mark-in-progress 1
todo mark-done 1
todo delete 1
```

## Task Status

```text
todo
in-progress
done
```

Each task contains:

```text
id
description
status
createdAt
updatedAt
```

## Project Structure

```text
Commands/    CLI commands
Handlers/    Application logic
Store/       JSON persistence
Core/        Domain models and results
UI/          Console output
Tests/       Unit and integration tests
```


## Roadmap.sh Project

This project was built as part of the Roadmap.sh [Roadmap.sh Task Tracker](https://roadmap.sh/projects/task-tracker) project
