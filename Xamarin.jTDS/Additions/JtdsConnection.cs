using Android.Bluetooth;

namespace Net.Sourceforge.Jtds.Jdbc
{
	public partial class JtdsConnection 
	{
		public System.Collections.Generic.IDictionary<string,Java.Lang.Class> TypeMap { 
			get { 
				getTypeMap(); 
				return new System.Collections.Generic.Dictionary<string, Java.Lang.Class> ();
			}
			set { 
				setTypeMap (new System.Collections.Generic.Dictionary<string, Java.Lang.Class> ());
			} 
		}

	}
}

