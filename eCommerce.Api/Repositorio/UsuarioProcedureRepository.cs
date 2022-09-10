﻿using eCommerce.Api.Models;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace eCommerce.Api.Repositorio
{
    public class UsuarioProcedureRepository
    {
        private IDbConnection _connection;

        public UsuarioProcedureRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public List<Usuario> Get()
        {
            List<Usuario> usuarios = new List<Usuario>();
            try
            {
                SqlCommand command = new SqlCommand();
                command.Connection = (SqlConnection)_connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SelecionarUsuarios";
                _connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    Usuario usuario = new Usuario();
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
                    usuarios.Add(usuario);
                }

            }
            finally
            {
                _connection.Close();
            }

            return usuarios;
        }

        public Usuario Get(int id)
        {
            try
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = "SelecionarUsuario";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", id);
                command.Connection = (SqlConnection)_connection;

                _connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    Usuario usuario = new Usuario();
                    usuario.Id = dataReader.GetInt32(0);
                    usuario.Nome = dataReader.GetString("Nome");
                    usuario.Email = dataReader.GetString("Email");
                    usuario.Sexo = dataReader.GetString("Sexo");
                    usuario.RG = dataReader.GetString("RG");
                    usuario.Cpf = dataReader.GetString("CPF");
                    usuario.NomeMae = dataReader.GetString("NomeMae");
                    usuario.SituacaoCadastro = dataReader.GetString("SituacaoCadastro");
                    usuario.DataCadastro = dataReader.GetDateTimeOffset(8);

                    return usuario;
                }
            }
            finally
            {
                _connection.Close();
            }

            return null;
        }

        public void Insert(Usuario usuario)
        {
            _connection.Open();
            try
            {
                SqlCommand command = new SqlCommand();
                command.Connection = (SqlConnection)_connection;
                command.CommandText = "CadastrarUsuario";
                command.CommandType = CommandType.StoredProcedure;


                command.Parameters.AddWithValue("@nome", usuario.Nome);
                command.Parameters.AddWithValue("@email", usuario.Email);
                command.Parameters.AddWithValue("@sexo", usuario.Sexo);
                command.Parameters.AddWithValue("@rg", usuario.RG);
                command.Parameters.AddWithValue("@cpf", usuario.Cpf);
                command.Parameters.AddWithValue("@nomeMae", usuario.NomeMae);
                command.Parameters.AddWithValue("@situacaoCadastro", usuario.SituacaoCadastro);
                command.Parameters.AddWithValue("@dataCadastro", usuario.DataCadastro);


                usuario.Id = (int)command.ExecuteScalar();
            }
            finally
            {
                _connection.Close();
            }
        }

        public void Update(Usuario usuario)
        {
            _connection.Open();

            try
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = "AtualizarUsuario";
                command.CommandType = CommandType.StoredProcedure;
                command.Connection = (SqlConnection)_connection;

                command.Parameters.AddWithValue("@nome", usuario.Nome);
                command.Parameters.AddWithValue("@email", usuario.Email);
                command.Parameters.AddWithValue("@sexo", usuario.Sexo);
                command.Parameters.AddWithValue("@rg", usuario.RG);
                command.Parameters.AddWithValue("@cpf", usuario.Cpf);
                command.Parameters.AddWithValue("@nomeMae", usuario.NomeMae);
                command.Parameters.AddWithValue("@situacaoCadastro", usuario.SituacaoCadastro);
                command.Parameters.AddWithValue("@dataCadastro", usuario.DataCadastro);

                command.Parameters.AddWithValue("@id", usuario.Id);

                command.ExecuteNonQuery();
            }
            finally
            {
                _connection.Close();
            }
        }

        public void Delete(int id)
        {
            try
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = "DeletarUsuario";
                command.CommandType = CommandType.StoredProcedure;
                command.Connection = (SqlConnection)_connection;

                command.Parameters.AddWithValue("@id", id);

                _connection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
