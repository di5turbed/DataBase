using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApi.Data;
using System.Data.Common;

namespace ServerApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SqlController : ControllerBase
    {
        private readonly EsportsContext _context;

        public SqlController(EsportsContext context)
        {
            _context = context;
        }

        // Класс для принятия SQL-строки от клиента
        public class SqlQueryRequest
        {
            public string Query { get; set; } = string.Empty;
        }

        [HttpPost("execute")]
        public async Task<IActionResult> ExecuteRawSql([FromBody] SqlQueryRequest request)
        {
            try
            {
                // Используем ADO.NET для динамического чтения базы данных
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = request.Query;

                using var reader = await command.ExecuteReaderAsync();

                // Формируем динамический список словарей (Колонка -> Значение)
                var results = new List<Dictionary<string, object>>();

                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var columnName = reader.GetName(i);
                        var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        row[columnName] = value ?? "NULL";
                    }
                    results.Add(row);
                }

                await connection.CloseAsync();
                return Ok(results); // Возвращаем динамический JSON
            }
            catch (Exception ex)
            {
                // Если в SQL ошибка (опечатка), возвращаем её клиенту, чтобы он понял, что не так
                return BadRequest(ex.Message);
            }
        }
    }
}