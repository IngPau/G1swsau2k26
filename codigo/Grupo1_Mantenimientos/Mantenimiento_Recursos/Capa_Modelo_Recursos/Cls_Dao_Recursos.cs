using System;
using System.Data;
using System.Data.Odbc;
using Capa_Modelo_Navegador;

//Inicio de código de "[Nombre del programador]" con carné: "[No_carné]" en la fecha de: "08/10/2026"
namespace Capa_Modelo_Recursos
{
    /// <summary>
    /// DAO de la tabla tbl_recursos. Reutiliza la conexión ODBC (DSN bd_auditoria)
    /// del componente Navegador. Todas las consultas son parametrizadas.
    /// </summary>
    public class Cls_Dao_Recursos
    {
        private readonly Cls_ConexionMYSQL gConexion = new Cls_ConexionMYSQL();

        /// <summary>Lista los recursos con el nombre de su proyecto.</summary>
        public DataTable fun_listar_recursos()
        {
            string sSql =
                "SELECT r.Pk_Id_Recurso, r.Fk_Id_Proyecto, p.Cmp_Nombre_Proyecto, " +
                "r.Cmp_Nombre_Recurso, r.Cmp_Tipo_Recurso, r.Cmp_Cantidad_Recurso, " +
                "r.Cmp_Fecha_Registro_Recurso " +
                "FROM tbl_recursos r " +
                "INNER JOIN tbl_proyecto p ON p.Pk_Id_Proyecto = r.Fk_Id_Proyecto " +
                "ORDER BY r.Pk_Id_Recurso";

            DataTable dtsRecursos = new DataTable();
            using (OdbcConnection conn = gConexion.conexion())
            {
                conn.Open();
                using (OdbcDataAdapter adaptador = new OdbcDataAdapter(sSql, conn))
                {
                    adaptador.Fill(dtsRecursos);
                }
            }
            return dtsRecursos;
        }

        /// <summary>Lista los proyectos para llenar el combo de la vista.</summary>
        public DataTable fun_listar_proyectos()
        {
            string sSql =
                "SELECT Pk_Id_Proyecto, Cmp_Nombre_Proyecto " +
                "FROM tbl_proyecto ORDER BY Cmp_Nombre_Proyecto";

            DataTable dtsProyectos = new DataTable();
            using (OdbcConnection conn = gConexion.conexion())
            {
                conn.Open();
                using (OdbcDataAdapter adaptador = new OdbcDataAdapter(sSql, conn))
                {
                    adaptador.Fill(dtsProyectos);
                }
            }
            return dtsProyectos;
        }

        /// <summary>
        /// Inserta un recurso y devuelve su nuevo id.
        /// Pk_Id_Recurso aún no es AUTO_INCREMENT en la BD, por eso el id se calcula
        /// dentro de una transacción (FOR UPDATE evita que dos usuarios tomen el mismo).
        /// Si la BD agrega AUTO_INCREMENT, este método sigue funcionando igual.
        /// </summary>
        public int fun_insertar_recurso(int iIdProyecto, string sNombre, string sTipo, int iCantidad)
        {
            using (OdbcConnection conn = gConexion.conexion())
            {
                conn.Open();
                using (OdbcTransaction trx = conn.BeginTransaction())
                {
                    int iNuevoId;
                    using (OdbcCommand cmdId = new OdbcCommand(
                        "SELECT COALESCE(MAX(Pk_Id_Recurso), 0) + 1 FROM tbl_recursos FOR UPDATE", conn, trx))
                    {
                        iNuevoId = Convert.ToInt32(cmdId.ExecuteScalar());
                    }

                    using (OdbcCommand cmdInsertar = new OdbcCommand(
                        "INSERT INTO tbl_recursos " +
                        "(Pk_Id_Recurso, Fk_Id_Proyecto, Cmp_Nombre_Recurso, Cmp_Tipo_Recurso, Cmp_Cantidad_Recurso) " +
                        "VALUES (?, ?, ?, ?, ?)", conn, trx))
                    {
                        cmdInsertar.Parameters.AddWithValue("@id", iNuevoId);
                        cmdInsertar.Parameters.AddWithValue("@proyecto", iIdProyecto);
                        cmdInsertar.Parameters.AddWithValue("@nombre", sNombre);
                        cmdInsertar.Parameters.AddWithValue("@tipo", sTipo);
                        cmdInsertar.Parameters.AddWithValue("@cantidad", iCantidad);
                        cmdInsertar.ExecuteNonQuery();
                    }

                    trx.Commit();
                    return iNuevoId;
                }
            }
        }

        /// <summary>Modifica un recurso. Devuelve las filas afectadas.</summary>
        public int fun_modificar_recurso(int iIdRecurso, int iIdProyecto, string sNombre, string sTipo, int iCantidad)
        {
            using (OdbcConnection conn = gConexion.conexion())
            {
                conn.Open();
                using (OdbcCommand cmdModificar = new OdbcCommand(
                    "UPDATE tbl_recursos SET Fk_Id_Proyecto = ?, Cmp_Nombre_Recurso = ?, " +
                    "Cmp_Tipo_Recurso = ?, Cmp_Cantidad_Recurso = ? " +
                    "WHERE Pk_Id_Recurso = ?", conn))
                {
                    cmdModificar.Parameters.AddWithValue("@proyecto", iIdProyecto);
                    cmdModificar.Parameters.AddWithValue("@nombre", sNombre);
                    cmdModificar.Parameters.AddWithValue("@tipo", sTipo);
                    cmdModificar.Parameters.AddWithValue("@cantidad", iCantidad);
                    cmdModificar.Parameters.AddWithValue("@id", iIdRecurso);
                    return cmdModificar.ExecuteNonQuery();
                }
            }
        }

        /// <summary>Elimina un recurso. Devuelve las filas afectadas.</summary>
        public int fun_eliminar_recurso(int iIdRecurso)
        {
            using (OdbcConnection conn = gConexion.conexion())
            {
                conn.Open();
                using (OdbcCommand cmdEliminar = new OdbcCommand(
                    "DELETE FROM tbl_recursos WHERE Pk_Id_Recurso = ?", conn))
                {
                    cmdEliminar.Parameters.AddWithValue("@id", iIdRecurso);
                    return cmdEliminar.ExecuteNonQuery();
                }
            }
        }
    }
}
//Fin del código de "[Nombre del programador]" con carné: "[No_carné]" en la fecha de : "08/10/2026"
