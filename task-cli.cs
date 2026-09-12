#!/usr/bin/env -S dotnet --

#:package System.CommandLine@2.0.12
#:package Spectre.Console@0.57.0

#:include Models/*.cs
#:include Core/*.cs
#:include Serialization/*.cs
#:include Store/*.cs
#:include Services/*.cs
#:include Handlers/*.cs
#:include UI/*.cs
#:include Commands/*.cs

using System.CommandLine;

var store = new JsonTaskStore();

var addHandler = new AddTaskHandler(store);
var listHandler = new ListTasksHandler(store);
var updateHandler = new UpdateTaskHandler(store);
var deleteHandler = new DeleteTaskHandler(store);
var changeStatusHandler = new ChangeTaskStatusHandler(store);

var rootCommand = new RootCommand(
    "A simple Task Tracker CLI.");

rootCommand.Subcommands.Add(
    AddCommand.Create(addHandler));

rootCommand.Subcommands.Add(
    ListCommand.Create(listHandler));

rootCommand.Subcommands.Add(
    UpdateCommand.Create(updateHandler));

rootCommand.Subcommands.Add(
    DeleteCommand.Create(deleteHandler));

rootCommand.Subcommands.Add(
    MarkDoneCommand.Create(changeStatusHandler));

rootCommand.Subcommands.Add(
    MarkInProgressCommand.Create(changeStatusHandler));

rootCommand.Subcommands.Add(
    MarkTodoCommand.Create(changeStatusHandler));

return await rootCommand.Parse(args).InvokeAsync();