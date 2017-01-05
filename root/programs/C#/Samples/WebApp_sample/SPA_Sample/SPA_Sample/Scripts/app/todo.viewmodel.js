﻿window.todoApp.todoListViewModel = (function (ko, datacontext) {
    /// <field name="todoLists" value="[new datacontext.todoList()]"></field>
    var todoLists = ko.observableArray(),
        error = ko.observable(),
        addTodoList = function () {
            var todoList = datacontext.createTodoList();
            todoList.isEditingListTitle(true);
            datacontext.saveNewTodoList(todoList)
                .then(addSucceeded)
                .fail(addFailed);

            function addSucceeded() {
                showTodoList(todoList);
            }
            function addFailed() {
                error("新しい todoList の保存に失敗しました");
            }
        },
        showTodoList = function (todoList) {
            todoLists.unshift(todoList); // Insert new todoList at the front
        },
        deleteTodoList = function (todoList) {
            todoLists.remove(todoList);
            datacontext.deleteTodoList(todoList)
                .fail(deleteFailed);

            function deleteFailed() {
                showTodoList(todoList); // re-show the restored list
            }
        };

    datacontext.getTodoLists(todoLists, error); // load todoLists

    return {
        todoLists: todoLists,
        error: error,
        addTodoList: addTodoList,
        deleteTodoList: deleteTodoList
    };

})(ko, todoApp.datacontext);

// Initiate the Knockout bindings
ko.applyBindings(window.todoApp.todoListViewModel);
