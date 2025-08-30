using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PlatformSchool.Domain.Base;
using PlatformSchool.Domain.Models.Department;
using PlatformSchool.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PlatformSchool.Persistence.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DepartmentRepository> _logger;
        private readonly string _connectionString;

        public DepartmentRepository(IConfiguration configuration,
                                    ILogger<DepartmentRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("connSchooldb");
        }
        public async Task<OperationResult<DepartmentUpdateModel>> CreateDepartmentAsync(DepartmentCreateModel model)
        {
            OperationResult<DepartmentUpdateModel> result = new OperationResult<DepartmentUpdateModel>();

            try
            {
                // validar los datos del modelo //

                if (model is null)
                {
                    result.IsSuccess = false;
                    result.Message = "The department model is null.";
                    return result;
                }

                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
                {
                    return OperationResult<DepartmentUpdateModel>.Failure("The department name is required.");
                }

                if (model.Name.Length > 50)
                {
                    return OperationResult<DepartmentUpdateModel>.Failure("The department name must not exceed 50 characters.");
                }

                if (model.Budget <= 0)
                {
                    return OperationResult<DepartmentUpdateModel>.Failure("The department budget must be greater than zero.");
                }

                _logger.LogInformation($"Creating a new department: {model.Name}");

                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand("[dbo].[AgregarDepartamento]", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_Name", model.Name);
                        command.Parameters.AddWithValue("@p_Budget", model.Budget);
                        command.Parameters.AddWithValue("@p_StartDate", model.StartDate);
                        command.Parameters.AddWithValue("@p_Administrator", model.Administrator);
                        command.Parameters.AddWithValue("@p_CreateUser", model.CreationUser);

                        SqlParameter p_result = new SqlParameter("@p_result", System.Data.SqlDbType.VarChar)
                        {
                            Size = 1000,
                            Direction = System.Data.ParameterDirection.Output
                        };

                        command.Parameters.Add(p_result);

                        await connection.OpenAsync();

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        var resultMessage = p_result.Value.ToString();

                        if (rowsAffected > 0)
                        {

                            _logger.LogInformation($"Department created successfully: {model.Name}. Result: {resultMessage}");
                            var createdDepartment = new DepartmentUpdateModel
                            {
                                Name = model.Name,
                                Budget = model.Budget,
                                StartDate = model.StartDate,
                                Administrator = model.Administrator
                            };
                            result = OperationResult<DepartmentUpdateModel>.Success("Department created successfully.", createdDepartment);
                        }
                        else
                        {

                            _logger.LogWarning($"No rows were affected when creating the department: {model.Name}. Result: {resultMessage}");
                            result = OperationResult<DepartmentUpdateModel>.Failure("No rows were affected when creating the department.");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the department.");
                result = OperationResult<DepartmentUpdateModel>.Failure($"An error occurred while creating the department: {ex.Message}");
            }

            return result;

        }

        public async Task<OperationResult<List<DepartmentGetModel>>> GetAllDepartmentsAsync()
        {
            OperationResult<List<DepartmentGetModel>> result = new OperationResult<List<DepartmentGetModel>>();

            try
            {
                using (var connetction = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand("[dbo].[ObtenerDepartamentos]", connetction))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        await connetction.OpenAsync();


                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            var departments = new List<DepartmentGetModel>();

                            while (await reader.ReadAsync())
                            {

                                var department = new DepartmentGetModel
                                {

                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Budget = reader.GetDecimal(reader.GetOrdinal("Budget")),
                                    StartDate = reader.GetString("StartDate"),
                                    Administrator = reader.IsDBNull(reader.GetOrdinal("Administrator")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Administrator"))
                                };
                                departments.Add(department);
                            }


                            if (departments.Any())
                            {
                                result = OperationResult<List<DepartmentGetModel>>.Success("Departments retrieved successfully.", departments);
                            }
                            else
                            {
                                result = OperationResult<List<DepartmentGetModel>>.Failure("No departments found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while retrieving departments.");
                result = OperationResult<List<DepartmentGetModel>>.Failure($"An error occurred while retrieving departments: {ex.Message}");
            }
            return result;

        }

        public async Task<OperationResult<DepartmentGetModel>> GetDepartmentByIdAsync(int id)
        {
            OperationResult<DepartmentGetModel> result = new OperationResult<DepartmentGetModel>();

            try
            {
                using (var connetction = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand("[dbo].[ObtenerDepartamentos]", connetction))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        await connetction.OpenAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                DepartmentGetModel departmentFound = new DepartmentGetModel();

                                while (await reader.ReadAsync())
                                {

                                    departmentFound.DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"));
                                    departmentFound.Name = reader.GetString(reader.GetOrdinal("Name"));
                                    departmentFound.Budget = reader.GetDecimal(reader.GetOrdinal("Budget"));
                                    departmentFound.StartDate = reader.GetString(reader.GetOrdinal("StartDate"));
                                    departmentFound.Administrator = reader.IsDBNull(reader.GetOrdinal("Administrator")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Administrator"));


                                }

                                result = OperationResult<DepartmentGetModel>.Success("Departments retrieved successfully.",departmentFound);

                            }
                            else
                            {
                                result = OperationResult<DepartmentGetModel>.Failure("No department found.");

                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while retrieving department.");
                result = OperationResult<DepartmentGetModel>.Failure($"An error occurred while retrieving department: {ex.Message}");

            }
            return result;
        }

        public async Task<OperationResult<DepartmentUpdateModel>> UpdateDepartmentAsync(DepartmentUpdateModel model)
        {
            OperationResult<DepartmentUpdateModel> result = new OperationResult<DepartmentUpdateModel>();

            try
            {

                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand("[dbo].[ActualizarDepartamento]", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@@DepartmentId", model.DepartmentId);
                        command.Parameters.AddWithValue("@Name", model.Name);
                        command.Parameters.AddWithValue("@Budget", model.Budget);
                        command.Parameters.AddWithValue("@StartDate", model.StartDate);
                        command.Parameters.AddWithValue("@Administrator", model.Administrator);
                        command.Parameters.AddWithValue("@UserMod", model.UserMod);

                        SqlParameter p_result = new SqlParameter("@p_result", System.Data.SqlDbType.VarChar)
                        {
                            Size = 1000,
                            Direction = System.Data.ParameterDirection.Output
                        };

                        command.Parameters.Add(p_result);

                        await connection.OpenAsync();

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        var resultMessage = p_result.Value.ToString();

                        if (rowsAffected > 0)
                        {

                            _logger.LogInformation($"Department created successfully: {model.Name}. Result: {resultMessage}");
                            var createdDepartment = new DepartmentUpdateModel
                            {
                                Name = model.Name,
                                Budget = model.Budget,
                                StartDate = model.StartDate,
                                Administrator = model.Administrator
                            };
                            result = OperationResult<DepartmentUpdateModel>.Success("Department created successfully.", createdDepartment);
                        }
                        else
                        {

                            _logger.LogWarning($"No rows were affected when creating the department: {model.Name}. Result: {resultMessage}");
                            result = OperationResult<DepartmentUpdateModel>.Failure("No rows were affected when creating the department.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the department.");
                result = OperationResult<DepartmentUpdateModel>.Failure($"An error occurred while updating the department: {ex.Message}");

            }
            return result;
        }
    }
}
