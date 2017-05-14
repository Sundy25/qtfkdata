using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SqlClient;
using QTFK.Services.DBIO;
using QTFK.Services;
using QTFK.Extensions.DBIO.DBQueries;
using QTFK.Extensions.DBIO;
using System.Linq;
using System.Collections.Generic;
using QTFK.Services.Factories;
using QTFK.Extensions.DBIO.CRUDFactory;

namespace QTFK.Data.Tests
{
    [TestClass]
    public class CrudFactoryTests
    {
        [TestMethod]
        public void TestMethod1()
        {

        }

        bool ConsumerExampleUsingQTFK(string id, string subID)
        {
            var factory = new CRUDFactory()
                .Register<ISQLServer>(() => new SqlServerCrudFactory(new SQLServerDBIO("")), true)
                .Register<IOleDB>(() => new OleDBCrudFactory(new OleDBIO("")), false)
                ;

            var dbio = factory.Get<ISQLServer>();

            //this select is independent of SQL engine
            var actuaciones = dbio.Select<Actuacion>(q => q
                .Select("vActuaciones", c => c.Column("IdFDTT").Column("IdPS").Column("RevisadaPor"))
                .SetWhere($@" IdFDTT='{id}' AND IdPS='{subID}' "))
                .ToList()
                ;

            if (!actuaciones.Any())
                return true;
            else
            {
                dbio.Update(q => q
                    .Set("Actuaciones", c => c.Column("EstadoTablaActuaciones", "APROBADO"))
                    .SetWhere($" IdFDTT = {id} AND IdPS = {subID}"));
                return false;
            }
        }

        bool ConsumerExampleMethod(string id, string subID)
        {
            bool result;
            String query = "SELECT IdFDTT, IdPS, RevisadaPor"
                            + " FROM vActuaciones"
                            + $" WHERE IdFDTT='{id}' AND IdPS='{subID}' "
                            ;

            DataTable dt = DB_Select(query);
            if (dt == null)
            {
                result = false;
            }

            IList<Actuacion> actuaciones = new List<Actuacion>();
            foreach (var row in dt.AsEnumerable())
            {
                actuaciones.Add(new Actuacion
                {
                    IdFDTT = (string)row["IdFDTT"],
                    IdPS = (string)row["IdPS"],
                    RevisadaPor = (string)row["RevisadaPor"],
                });
            }
            if (actuaciones.Count == 0)
            {
                result = true;
            }
            else
            {
                string sql_insert = "UPDATE Actuaciones"
                    + " SET EstadoTablaActuaciones = 'APROBADO'"
                    + $" WHERE IdFDTT = {id} AND IdPS = {subID}"
                    ;
                int affected = DB_Execute(sql_insert);
                if (affected != 1)
                {
                    //Logger.log.AddError("MoveApprovedActions", null, $"Fallo al poner estado APROBADO a la actuacion IdFDTT:{id}");
                }
                else
                {
                    //Logger.log.AddMensaje("MoveApprovedAction", null, $"Actuacion IdFDTT:{id} aprobada con exito!!");
                }
                result = true;
            }
            if (dt != null)
            {
                dt.Dispose();
            }
            return result;
        }

        private static string _dbConnetionString = "";

public static DataTable DB_Select(string query)
{
    DataTable dataTable = null;

    using (SqlConnection connection = new System.Data.SqlClient.SqlConnection(_dbConnetionString))
    using (SqlCommand command = new SqlCommand())
    {
        command.Connection = connection;            // <== lacking
        command.CommandType = CommandType.Text;
        command.CommandText = query;
        try
        {
            connection.Open();

            SqlDataReader dataReader = command.ExecuteReader();
            dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataReader.Close();
        }
        catch (Exception ex)
        {
            if (dataTable != null)
            {
                dataTable.Dispose();
                dataTable = null;
            }
        }
    }

    return dataTable;
}

        public static int DB_Execute(string query, SqlParameter[] parameters = null)
        {
            int affectedRows = 0;
            using (SqlConnection connection = new SqlConnection(_dbConnetionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = query;

                try
                {
                    connection.Open();
                    affectedRows = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }

            return affectedRows;
        }


        public static long DB_ExecuteInsert(string query, string tabla = "")
        {
            long insertedID = 0;
            using (SqlConnection connection = new SqlConnection(_dbConnetionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;            // <== lacking
                        command.CommandType = CommandType.Text;
                        command.CommandText = query + ";SELECT SCOPE_IDENTITY() AS ID";
                        try
                        {
                            string newId = command.ExecuteScalar().ToString();
                            insertedID = Convert.ToInt64(newId);
                        }
                        catch (SqlException ex)
                        {
                            //Logger.log.AddError("DB_ExecuteInsert", null, "exception:" + ex.Message);
                        }
                    }
                    if (tabla != "")
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;            // <== lacking
                            command.CommandType = CommandType.Text;
                            //command.CommandText = "SELECT MAX(IdTablaDocumentosFDTT) FROM DocumentosFDTT";
                            command.CommandText = "SELECT IDENT_CURRENT ('" + tabla + "') AS Current_Identity;";
                            try
                            {
                                DataTable dataTable = new DataTable();

                                SqlDataReader dataReader = command.ExecuteReader();
                                dataTable.Load(dataReader);
                                dataReader.Close();

                                DataRow dbRow = dataTable.Rows[0];
                                insertedID = dbRow.Field<long>(0);

                                dataTable.Dispose();
                            }
                            catch (SqlException ex)
                            {
                                //Logger.log.AddError("DB_ExecuteInsert", null, " Exception: " + ex.Message);
                            }
                            finally
                            {
                                command.Dispose();
                            }
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    //Logger.log.AddError("DB_ExecuteInsert", null, "exception:" + ex.Message);
                }
            }

            return insertedID;
        }
    };

    public class Actuacion
    {
        public string IdFDTT { get; set; }
        public string IdPS { get; set; }
        public string RevisadaPor { get; set; }
    }
}
