using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Todo.Models;
using Todo.Models.ViewModels;

namespace Todo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var todoListViewModel = GetAllTodos();
        return View(todoListViewModel);
    }

    public JsonResult PopulateForm(int id)
    {
        var todo = GetById(id);
        return Json(todo);
    }

    internal TodoViewModel GetAllTodos()
    {
        List<TodoModel> todoList = new();

        using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = "SELECT * FROM todo";

                using (var reader = tableCmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            todoList.Add(
                                new TodoModel
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                });
                        }
                        return new TodoViewModel
                        {
                            TodoList = todoList
                        };
                    }
                    else
                    {
                        return new TodoViewModel
                        {
                            TodoList = todoList
                        };
                    }
                };
            }
        }
    }

    internal TodoModel GetById(int id)
    {
        TodoModel todo = new();

        using (var con = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = $"SELECT * FROM todo WHERE Id = '{id}'";

                using (var reader = tableCmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        todo.Id = reader.GetInt32(0);
                        todo.Name = reader.GetString(1);
                    }
                    else
                    {
                        return todo;
                    }
                };
            }
        }
        return todo;
    }

    public RedirectResult Insert(TodoModel todo)
    {
        using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = $"INSERT INTO todo (name) VALUES ('{todo.Name}')";

                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        return Redirect("http://localhost:5075/");
    }

    public RedirectResult Update(TodoModel todo)
    {
        using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = $"UPDATE todo SET name = '{todo.Name}' WHERE Id = '{todo.Id}'";

                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        return Redirect("http://localhost:5075/");
    }

    public JsonResult Delete(int id)
    {
        using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = $"DELETE FROM todo WHERE Id = '{id}'";
                tableCmd.ExecuteNonQuery();
            }
        }
        return Json(new { });
    }
}
