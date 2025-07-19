using UnityEngine;
using System;
using System.Data;
using System.Text;

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

using MySql.Data;
using Mirror;
using MySql.Data.MySqlClient;
using Zenject;
using UnityEngine.Events;
using TMPro;

namespace Minicop.Game.GravityRave
{
	public class DatabaseHandler : MonoBehaviour
	{
		public UnityEvent<NetworkIdentity> OnRegisterDenied = new UnityEvent<NetworkIdentity>();
		public UnityEvent<NetworkIdentity> OnRegisterAllowed = new UnityEvent<NetworkIdentity>();
		public UnityEvent<NetworkIdentity> OnEnterDenied = new UnityEvent<NetworkIdentity>();
		public UnityEvent<NetworkIdentity> OnEnterAllowed = new UnityEvent<NetworkIdentity>();
		[Inject]
		public NetworkManager _networkManager;
		private MySqlConnection _conn = null;
		public int Id;

		private MD5 _md5Hash;

		void Start()
		{
			JSONController.Load(ref Data, "DatabaseHandlerData");

#if UNITY_EDITOR || DEVELOPMENT_BUILD

			if (!Data.IsActive) return;


			DataBaseOpen();

			try
			{
				MySqlCommand cmd = _conn.CreateCommand();
				cmd.CommandText = "select * from users;";

				MySqlDataReader rdr = cmd.ExecuteReader();
				if (rdr.HasRows)
				{
					while (rdr.Read())
					{
						int id = rdr.GetInt32("Id");
						string login = rdr.GetString("Login");
						string email = rdr.GetString("Email");
						string password = rdr.GetString("Password");

						Debug.Log($"LoadData: {id} {email}, {password}, {login}");
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Log($"{ex.ToString()}");
			}

			DataBaseClose();
#endif
		}
		public void OnApplicationQuit()
		{
			DataBaseClose();
		}

		private void DataBaseOpen()
		{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
			try
			{
				_conn = new MySqlConnection($"server={Data.IpAdress};uid={Data.UserName};pwd={Data.UserPassword};database={Data.Name}");
				Debug.Log($"Connecting...");
				_conn.Open();
			}
			catch (Exception ex)
			{
				Debug.Log($"{ex.ToString()}");
			}
#endif
		}
		public void DataBaseClose()
		{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
			try
			{
				if (_conn != null)
				{
					if (_conn.State.ToString() != "Closed")
					{
						_conn.Close();
						Debug.Log($"Connect closed");
					}
					_conn.Dispose();
				}
			}
			catch (Exception ex)
			{
				Debug.Log($"{ex.ToString()}");
			}
#endif
		}

		public void CheckUser(NetworkIdentity networkIdentity, string email, string password, string login)
		{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
			DataBaseOpen();

			try
			{
				MySqlCommand cmd = _conn.CreateCommand();
				cmd.CommandText = $"SELECT COUNT(*) AS IsBusy FROM users WHERE Email = '{email}' AND Login = '{login}' AND Password = '{password}';";

				MySqlDataReader rdr = cmd.ExecuteReader();
				if (rdr.HasRows)
				{
					while (rdr.Read())
					{
						int count = rdr.GetInt32("IsBusy");
						if (count != 0)
						{
							Debug.Log("Login, Email, and Password correct");
							OnRegisterAllowed.Invoke(networkIdentity);
							DataBaseClose();
							return;
						}
						else
						{
							Debug.Log($"Login, Email, or Password incorrect");
							DataBaseClose();
							return;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Log($"{ex.ToString()}");
			}
			DataBaseClose();
#endif
		}

		[Server]
		public void SaveUser(NetworkIdentity networkIdentity, string email, string password, string login)
		{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
			DataBaseOpen();

			try
			{
				MySqlCommand cmd = _conn.CreateCommand();
				cmd.CommandText = $"SELECT COUNT(*) AS IsBusy FROM users WHERE Email = '{email}' OR Login = '{login}';";

				MySqlDataReader rdr = cmd.ExecuteReader();
				if (rdr.HasRows)
				{
					while (rdr.Read())
					{
						int count = rdr.GetInt32("IsBusy");
						if (count != 0)
						{
							Debug.Log("Login or Email is busy");
							DataBaseClose();
							return;
						}
						else
							Debug.Log($"Login and Email not busy");
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Log($"{ex.ToString()}");
			}
			DataBaseClose();

			DataBaseOpen();
			try
			{
				Debug.Log($"Login and Email not busy");

				Debug.Log($"Data inserting...");

				string query = "INSERT INTO users(Email, Password, Login) VALUES (?Email, ?Password,?Login);";
				MySqlCommand cmd = new MySqlCommand(query, _conn);
				cmd.Parameters.Add("?Email", MySqlDbType.VarChar).Value = email;
				cmd.Parameters.Add("?Password", MySqlDbType.VarChar).Value = password;
				cmd.Parameters.Add("?Login", MySqlDbType.VarChar).Value = login;
				cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				Debug.Log($"{ex.ToString()}");
			}

			DataBaseClose();

			OnEnterAllowed.Invoke(networkIdentity);
#endif
		}

		public static DataStruct Data = new DataStruct();
		[System.Serializable]
		public struct DataStruct
		{
			public bool IsActive;
			public string IpAdress;
			public string Port;
			public string Name;
			public string UserName;
			public string UserPassword;
		}
	}

}