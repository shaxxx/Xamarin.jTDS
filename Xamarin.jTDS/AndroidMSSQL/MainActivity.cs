using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using Java.Sql;
using System.Threading;
using Net.Sourceforge.Jtds.Jdbc;

namespace AndroidMSSQL
{
	[Activity (Label = "AndroidMSSQL", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int count = 1;
		private Button _button;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			_button = button;
			button.Click += delegate {
				button.Text = string.Format ("Connect {0}!", count++);
				ThreadPool.QueueUserWorkItem (o => Connect ());
				button.Enabled = false;
			};
		}

		public void Connect ()
		{
			Java.Sql.IConnection con = null;
			try {
				var driver = new Net.Sourceforge.Jtds.Jdbc.Driver();
				String username = "username";
				String password = "password";
				String address = "192.168.1.101";
				String port = "1433";
				String database = "Database";					
				String connString = String.Format("jdbc:jtds:sqlserver://{0}:{1}/{2};user={3};password={4}",
					address, port, database, username, password);
				con = DriverManager.GetConnection (connString, username, password);
				IPreparedStatement stmt = null;
				try {
					//Prepared statement
					stmt = con.PrepareStatement ("SELECT * FROM Users WHERE Id = ? AND Name = ?");
					stmt.SetLong (1, 1);
					stmt.SetString (2, "John Doe");
					stmt.Execute ();

					RunOnUiThread (() => Toast.MakeText(this, "SUCCESS!", ToastLength.Short).Show());

					IResultSet rs = stmt.ResultSet;

					IResultSetMetaData rsmd = rs.MetaData;
					PrintColumnTypes.PrintColTypes (rsmd);
					Console.WriteLine ("");

					int numberOfColumns = rsmd.ColumnCount;

					for (int i = 1; i <= numberOfColumns; i++) {
						if (i > 1)
							Console.Write (",  ");
						String columnName = rsmd.GetColumnName (i);
						Console.Write (columnName);
					}
					Console.WriteLine ("");
					while (rs.Next ()) {
						for (int i = 1; i <= numberOfColumns; i++) {
							if (i > 1)
								Console.Write (",  ");
							String columnValue = rs.GetString (i);
							Console.Write (columnValue);
						}
						Console.WriteLine ("");  
					}

					stmt.Close ();
					con.Close ();
				} catch (Exception e) {
					RunOnUiThread (() => Toast.MakeText(this, e.Message, ToastLength.Long).Show());
					Console.WriteLine (e.StackTrace);
					stmt.Close ();
				}
				con.Close ();
			} catch (Exception e) {
				Console.WriteLine (e.StackTrace);
				RunOnUiThread (() => Toast.MakeText(this, e.Message, ToastLength.Long).Show());
			}
			RunOnUiThread (() => _button.Enabled = true);
		}			
	}


}


class PrintColumnTypes
{

	public static void PrintColTypes (IResultSetMetaData rsmd)
	{
		int columns = rsmd.ColumnCount;
		for (int i = 1; i <= columns; i++) {
			int jdbcType = rsmd.GetColumnType (i);
			String name = rsmd.GetColumnTypeName (i);
			Console.Write ("Column " + i + " is JDBC type " + jdbcType);
			Console.Write (", which the DBMS calls " + name + System.Environment.NewLine);
		}
	}
}