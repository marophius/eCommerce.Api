using eCommerce.Api.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace eCommerce.Api.Repositorio
{
    public class UsuarioRepository : IUsuarioRepositorio
    {
        private readonly IDbConnection _connection;

        public UsuarioRepository(IDbConnection connection)
        {
            _connection = connection;
        }


        public Usuario BuscarUsuario(int id)
        {
            try
            {
                SqlCommand select = new SqlCommand("SELECT *, c.Id AS ContatoId FROM Usuarios u LEFT JOIN Contatos c ON c.UsuarioId = u.Id LEFT JOIN EnderecosEntrega ee ON ee.UsuarioId = u.Id LEFT JOIN UsuariosDepartamentos ud ON ud.UsuarioId = u.Id LEFT JOIN Departamentos d ON ud.DepartamentoId = d.Id WHERE u.Id = @Id", (SqlConnection)_connection);
                select.Parameters.AddWithValue("@Id", id);
                _connection.Open();

                SqlDataReader dataReader = select.ExecuteReader();
                Usuario usuario = new Usuario();
                Dictionary<int, Usuario> usuarios = new Dictionary<int, Usuario>();

                while (dataReader.Read())
                {
                    if (!usuarios.ContainsKey(dataReader.GetInt32(0)))
                    {
                        usuario.Id = dataReader.GetInt32(0);
                        usuario.Nome = dataReader.GetString("Nome");
                        usuario.Email = dataReader.GetString("Email");
                        usuario.Sexo = dataReader.GetString("Sexo");
                        usuario.RG = dataReader.GetString("RG");
                        usuario.Cpf = dataReader.GetString("CPF");
                        usuario.NomeMae = dataReader.GetString("NomeMae");
                        usuario.SituacaoCadastro = dataReader.GetString("SituacaoCadastro");
                        usuario.DataCadastro = dataReader.GetDateTimeOffset(8);

                        var contato = new Contato();
                        contato.Id = dataReader.GetInt32("ContatoId");
                        contato.UsuarioId = usuario.Id;
                        contato.Telefone = dataReader.GetString("Telefone");
                        contato.Celular = dataReader.GetString("Celular");

                        usuario.Contato = contato;
                        usuarios.Add(usuario.Id, usuario);
                    } else
                    {
                        usuario = usuarios[dataReader.GetInt32(0)];
                    }


                    EnderecoEntrega endereco = new();
                    endereco.Id = dataReader.GetInt32(13);
                    endereco.UsuarioId = usuario.Id;
                    endereco.NomeEndereco = dataReader.GetString("NomeEndereco");
                    endereco.Cep = dataReader.GetString("CEP");
                    endereco.Estado = dataReader.GetString("Estado");
                    endereco.Bairro = dataReader.GetString("Bairro");
                    endereco.Cidade = dataReader.GetString("Cidade");
                    endereco.Numero = dataReader.GetString("Numero");
                    endereco.Complemento = dataReader.GetString("Complemento");

                    usuario.EnderecosEntrega = (usuario.EnderecosEntrega == null) ? new List<EnderecoEntrega>() : usuario.EnderecosEntrega;
                    if (usuario.EnderecosEntrega.FirstOrDefault(e => e.Id == endereco.Id) == null)
                    {
                        usuario.EnderecosEntrega.Add(endereco);
                    }

                    Departamento departamento = new();
                    departamento.Id = dataReader.GetInt32(26);
                    departamento.Nome = dataReader.GetString(27);
                    usuario.Departamentos = (usuario.Departamentos == null) ? new List<Departamento>() : usuario.Departamentos;

                    if (usuario.Departamentos.FirstOrDefault(d => d.Id == departamento.Id) == null)
                    {
                        usuario.Departamentos.Add(departamento);
                    }

                }


                return usuarios[usuarios.Keys.First()];

            } catch (Exception ex) 
            {
                return null;
            } finally
            {
                _connection.Close();
            }
        }

        public List<Usuario> BuscarUsuarios()
        {
            try
            {
                SqlCommand select = new SqlCommand("SELECT * FROM Usuarios", (SqlConnection)_connection);
                List<Usuario> list = new List<Usuario>();
                _connection.Open();
                SqlDataReader dataReader = select.ExecuteReader();

                while(dataReader.Read())
                {
                   Usuario usuario = new Usuario();
                    usuario.Id = dataReader.GetInt32("Id");
                    usuario.Nome = dataReader.GetString("Nome");
                    usuario.Email = dataReader.GetString("Email");
                    usuario.Sexo = dataReader.GetString("Sexo");
                    usuario.RG = dataReader.GetString("RG");
                    usuario.Cpf = dataReader.GetString("CPF");
                    usuario.NomeMae = dataReader.GetString("NomeMae");
                    usuario.SituacaoCadastro = dataReader.GetString("SituacaoCadastro");
                    usuario.DataCadastro = dataReader.GetDateTimeOffset(8);

                    list.Add(usuario);
                }

                return list;
            }finally
            {
                _connection.Close();
            }
            
        }

        public void DeleteUsuario(int id)
        {
            try
            {
                SqlCommand delete = new SqlCommand();
                delete.Connection = (SqlConnection)_connection;

                delete.CommandText = "DELETE FROM Usuarios WHERE Id = @Id";
                delete.Parameters.AddWithValue("@Id", id);

                _connection.Open();
                delete.ExecuteNonQuery();

            }finally
            {
                _connection.Close();
            }
        }

        public void InsertUsuario(Usuario usuario)
        {
            _connection.Open();
            SqlTransaction transaction = (SqlTransaction)_connection.BeginTransaction();
            try
            {
                SqlCommand insert = new SqlCommand();
                insert.CommandText = @"INSERT INTO Usuarios(
                                                        Nome, 
                                                        Email, 
                                                        Sexo, 
                                                        RG, 
                                                        CPF, 
                                                        NomeMae, 
                                                        SituacaoCadastro, 
                                                        DataCadastro
                                                        ) 
                                                        VALUES (
                                                            @Nome, 
                                                            @Email, 
                                                            @Sexo, 
                                                            @RG, 
                                                            @CPF, 
                                                            @NomeMae, 
                                                            @SituacaoCadastro, 
                                                            @DataCadastro); SELECT CAST(scope_identity() AS int)";
                insert.Connection = (SqlConnection)_connection;
                insert.Transaction = transaction;
                insert.Parameters.AddWithValue("@Nome", usuario.Nome);
                insert.Parameters.AddWithValue("@Email", usuario.Email);
                insert.Parameters.AddWithValue("@Sexo", usuario.Sexo);
                insert.Parameters.AddWithValue("@RG", usuario.RG);
                insert.Parameters.AddWithValue("@CPF", usuario.Cpf);
                insert.Parameters.AddWithValue("@NomeMae", usuario.NomeMae);
                insert.Parameters.AddWithValue("@SituacaoCadastro", usuario.SituacaoCadastro);
                insert.Parameters.AddWithValue("@DataCadastro", usuario.DataCadastro);

                usuario.Id = (int)insert.ExecuteScalar();

                insert.CommandText = @"INSERT INTO Contatos(UsuarioId, Telefone, Celular) VALUES (@UsuarioId, @Telefone, @Celular); SELECT CAST(scope_identity() AS int)";
                insert.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                insert.Parameters.AddWithValue("@Telefone", usuario.Contato.Telefone);
                insert.Parameters.AddWithValue("@Celular", usuario.Contato.Celular);

                usuario.Contato.UsuarioId = usuario.Id;
                usuario.Contato.Id = (int) insert.ExecuteScalar();

                foreach(var endereco in usuario.EnderecosEntrega)
                {
                    insert = new SqlCommand();
                    insert.Connection = (SqlConnection)_connection;
                    insert.Transaction = transaction;
                    insert.CommandText = @"INSERT INTO EnderecosEntrega (
                                                            UsuarioId, 
                                                            NomeEndereco, 
                                                            CEP, 
                                                            Estado, 
                                                            Cidade, 
                                                            Bairro, 
                                                            Endereco, 
                                                            Numero, 
                                                            Complemento
                                                            ) VALUES (
                                                            @UsuarioId, 
                                                            @NomeEndereco, 
                                                            @CEP, 
                                                            @Estado, 
                                                            @Cidade, 
                                                            @Bairro, 
                                                            @Endereco, 
                                                            @Numero, 
                                                            @Complemento
                                                            );";
                    insert.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                    insert.Parameters.AddWithValue("@NomeEndereco", endereco.NomeEndereco);
                    insert.Parameters.AddWithValue("@CEP", endereco.Cep);
                    insert.Parameters.AddWithValue("@Estado", endereco.Estado);
                    insert.Parameters.AddWithValue("@Cidade", endereco.Cidade);
                    insert.Parameters.AddWithValue("@Bairro", endereco.Bairro);
                    insert.Parameters.AddWithValue("@Endereco", endereco.Endereco);
                    insert.Parameters.AddWithValue("@Numero", endereco.Numero);
                    insert.Parameters.AddWithValue("@Complemento", endereco.Complemento);

                    endereco.Id = (int)insert.ExecuteScalar();
                    endereco.UsuarioId = usuario.Id;
                }

                foreach(var departamento in usuario.Departamentos)
                {
                    insert = new SqlCommand();
                    insert.Connection = (SqlConnection)_connection;
                    insert.Transaction = transaction;

                    insert.CommandText = @"INSERT INTO UsuariosDepartamentos (UsuarioId, DepartamentoId) VALUES (@UsuarioId, @DepartamentoId);";
                    insert.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                    insert.Parameters.AddWithValue("@DepartamentoId", departamento.Id);

                    insert.ExecuteNonQuery();
                }

                transaction.Commit();

            }catch(Exception ex)
            {
                try
                {
                    transaction.Rollback();

                }catch(Exception exe)
                {

                }
            }
            finally
            {
                _connection.Close();
            }
        }

        public void UpdateUsuario(Usuario usuario)
        {
            _connection.Open();
            SqlTransaction transaction = (SqlTransaction)_connection.BeginTransaction();
            try
            {
                SqlCommand update = new SqlCommand();
                update.CommandText = @"UPDATE Usuarios 
                                            SET
                                            Nome = @Nome,
                                            Email = @Email,
                                            Sexo = @Sexo,
                                            RG = @RG,
                                            CPF = @CPF,
                                            NomeMae = @NomeMae,
                                            SituacaoCadastro = @SituacaoCadastro,
                                            DataCadastro = @DataCadastro
                                            WHERE ID = @Id";
                update.Connection = (SqlConnection)_connection;
                update.Parameters.AddWithValue("@Id", usuario.Id);

                update.Parameters.AddWithValue("@Nome", usuario.Nome);
                update.Parameters.AddWithValue("@Email", usuario.Email);
                update.Parameters.AddWithValue("@Sexo", usuario.Sexo);
                update.Parameters.AddWithValue("@RG", usuario.RG);
                update.Parameters.AddWithValue("@CPF", usuario.Cpf);
                update.Parameters.AddWithValue("@NomeMae", usuario.NomeMae);
                update.Parameters.AddWithValue("@SituacaoCadastro", usuario.SituacaoCadastro);
                update.Parameters.AddWithValue("@DataCadastro", usuario.DataCadastro);

                
                update.ExecuteNonQuery();

                update = new SqlCommand();
                update.Connection = (SqlConnection)_connection;
                update.Transaction = transaction;
                update.CommandText = @"UPDATE Contatos SET UsuarioId = @UsuarioId, Telefone = @Telefone, Celular = @Celular WHERE Id = @Id";
                update.Parameters.AddWithValue("@UsuaioId", usuario.Id);
                update.Parameters.AddWithValue("@Telefone", usuario.Contato.Telefone);
                update.Parameters.AddWithValue("@Celular", usuario.Contato.Celular);
                update.Parameters.AddWithValue("@Id", usuario.Contato.Id);

                update.ExecuteNonQuery();


                update = new SqlCommand();
                update.Connection = (SqlConnection)_connection;
                update.Transaction = transaction;
                update.CommandText = @"DELETE FROM EnderecosEntrega WHERE UsuarioId = @UsuarioId";
                update.Parameters.Add(usuario.Id);

                update.ExecuteNonQuery();

                foreach (var endereco in usuario.EnderecosEntrega)
{
                    update = new SqlCommand();
                    update.Connection = (SqlConnection)_connection;
                    update.Transaction = transaction;
                    update.CommandText = @"INSERT INTO EnderecosEntrega (
                                                            UsuarioId, 
                                                            NomeEndereco, 
                                                            CEP, 
                                                            Estado, 
                                                            Cidade, 
                                                            Bairro, 
                                                            Endereco, 
                                                            Numero, 
                                                            Complemento
                                                            ) VALUES (
                                                            @UsuarioId, 
                                                            @NomeEndereco, 
                                                            @CEP, 
                                                            @Estado, 
                                                            @Cidade, 
                                                            @Bairro, 
                                                            @Endereco, 
                                                            @Numero, 
                                                            @Complemento
                                                            );"
;
                    update.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                    update.Parameters.AddWithValue("@NomeEndereco", endereco.NomeEndereco);
                    update.Parameters.AddWithValue("@CEP", endereco.Cep);
                    update.Parameters.AddWithValue("@Estado", endereco.Estado);
                    update.Parameters.AddWithValue("@Cidade", endereco.Cidade);
                    update.Parameters.AddWithValue("@Bairro", endereco.Bairro);
                    update.Parameters.AddWithValue("@Endereco", endereco.Endereco);
                    update.Parameters.AddWithValue("@Numero", endereco.Numero);
                    update.Parameters.AddWithValue("@Complemento", endereco.Complemento);

                    endereco.Id = (int)update.ExecuteScalar();
                    endereco.UsuarioId = usuario.Id;
                }

                update = new SqlCommand();
                update.Connection = (SqlConnection)_connection;
                update.Transaction = transaction;
                update.CommandText = @"DELETE FROM UsuariosDepartamentos WHERE UsuarioId = @UsuarioId";
                update.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                update.ExecuteNonQuery();

                foreach (var departamento in usuario.Departamentos)
                {
                    update = new SqlCommand();
                    update.Connection = (SqlConnection)_connection;
                    update.Transaction = transaction;

                    update.CommandText = @"INSERT INTO UsuariosDepartamentos (UsuarioId, DepartamentoId) VALUES (@UsuarioId, @DepartamentoId);";
                    update.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                    update.Parameters.AddWithValue("@DepartamentoId", departamento.Id);

                    update.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch(Exception ex)
            {
                try
                {
                    transaction.Rollback();
                }
                catch(Exception exc)
                {

                }
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
