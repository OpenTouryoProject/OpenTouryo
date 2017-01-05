window.todoApp = window.todoApp || {};

window.todoApp.datacontext = (function () {

    var datacontext = {
        getTodoLists: getTodoLists,
        createTodoItem: createTodoItem,
        createTodoList: createTodoList,
        saveNewTodoItem: saveNewTodoItem,
        saveNewTodoList: saveNewTodoList,
        saveChangedTodoItem: saveChangedTodoItem,
        saveChangedTodoList: saveChangedTodoList,
        deleteTodoItem: deleteTodoItem,
        deleteTodoList: deleteTodoList
    };

    return datacontext;

    function getTodoLists(todoListsObservable, errorObservable) {
        return ajaxRequest("get", todoListUrl())
            .done(getSucceeded)
            .fail(getFailed);

        function getSucceeded(data) {
            var mappedTodoLists = $.map(data, function (list) { return new createTodoList(list); });
            todoListsObservable(mappedTodoLists);
        }

        function getFailed() {
            errorObservable("Todo リストの取得中にエラーが発生しました。");
        }
    }
    function createTodoItem(data) {
        return new datacontext.todoItem(data); // todoItem is injected by todo.model.js
    }
    function createTodoList(data) {
        return new datacontext.todoList(data); // todoList is injected by todo.model.js
    }
    function saveNewTodoItem(todoItem) {
        clearErrorMessage(todoItem);
        return ajaxRequest("post", todoItemUrl(), todoItem)
            .done(function (result) {
                todoItem.todoItemId = result.todoItemId;
            })
            .fail(function () {
                todoItem.errorMessage("新しい Todo 項目の追加中にエラーが発生しました。");
            });
    }
    function saveNewTodoList(todoList) {
        clearErrorMessage(todoList);
        return ajaxRequest("post", todoListUrl(), todoList)
            .done(function (result) {
                todoList.todoListId = result.todoListId;
                todoList.userId = result.userId;
            })
            .fail(function () {
                todoList.errorMessage("新しい Todo リストの追加中にエラーが発生しました。");
            });
    }
    function deleteTodoItem(todoItem) {
        return ajaxRequest("delete", todoItemUrl(todoItem.todoItemId))
            .fail(function () {
                todoItem.errorMessage("Todo 項目の削除中にエラーが発生しました。");
            });
    }
    function deleteTodoList(todoList) {
        return ajaxRequest("delete", todoListUrl(todoList.todoListId))
            .fail(function () {
                todoList.errorMessage("Todo リストの削除中にエラーが発生しました。");
            });
    }
    function saveChangedTodoItem(todoItem) {
        clearErrorMessage(todoItem);
        return ajaxRequest("put", todoItemUrl(todoItem.todoItemId), todoItem, "text")
            .fail(function () {
                todoItem.errorMessage("Todo 項目の更新中にエラーが発生しました。");
            });
    }
    function saveChangedTodoList(todoList) {
        clearErrorMessage(todoList);
        return ajaxRequest("put", todoListUrl(todoList.todoListId), todoList, "text")
            .fail(function () {
                todoList.errorMessage("Todo リスト タイトルの更新中にエラーが発生しました。空でないことを確認してください。");
            });
    }

    // Private
    function clearErrorMessage(entity) { entity.errorMessage(null); }
    function ajaxRequest(type, url, data, dataType) { // Ajax helper
        var options = {
            dataType: dataType || "json",
            contentType: "application/json",
            cache: false,
            type: type,
            data: data ? data.toJson() : null
        };
        var antiForgeryToken = $("#antiForgeryToken").val();
        if (antiForgeryToken) {
            options.headers = {
                'RequestVerificationToken': antiForgeryToken
            }
        }
        return $.ajax(url, options);
    }
    // routes
    function todoListUrl(id) { return "/api/todolist/" + (id || ""); }
    function todoItemUrl(id) { return "/api/todo/" + (id || ""); }

})();