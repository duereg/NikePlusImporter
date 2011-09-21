using System;
using System.Collections.Generic;
using System.Text.RegularExpressions; 
using System.Text;

namespace Maximum.Helper
{
	/// <summary>
	/// Converts strings into their object representation
	/// </summary>
	/// <history>Comment Created:     [MDB]     3/4/2009 </history>
	public static class StringParser
	{
		private delegate object Converter(string obj);
		private static Dictionary<Type, Converter> _converterMap = new Dictionary<Type, Converter>();

		private static Converter GuidConverter = delegate(string toConvert)
		{
			return new Guid(toConvert);
		};
		private static Converter StringConverter = delegate(string toConvert)
		{
			return toConvert;
		};
		private static Converter IntegerConverter = delegate(string toConvert)
		{
			int retValue = int.MinValue + 1;

			if (!int.TryParse(toConvert, out retValue))
			{
				switch (toConvert.ToLower())
				{
					case "false":
					case "off":
					case "no":
						retValue = 0;
						break; 
					case "true":
					case "on":
					case "yes":
					case "checked":
						retValue = 1;
						break;
					default:
						retValue = int.MinValue + 1;
						break;
				}
			}
			if (retValue == int.MinValue + 1)
			{
				//No value found
				return null;
			}
			else
			{
				//value found
				return retValue;
			}
		};
		private static Converter DateConverter = delegate(string toConvert)
		{
			return DateTime.Parse(toConvert);
		};
		private static Converter BoolConverter = delegate(string toConvert)
		{
			bool retValue = false;

			if (!bool.TryParse(toConvert, out retValue))
			{
				switch (toConvert.ToLower())
				{
					case "true":
					case "on":
					case "yes":
					case "1":
					case "checked":
						retValue = true;
						break;
					default:
						retValue = false;
						break;
				}
			}

			return retValue;
		};
		private static Converter DecimalConverter = delegate(string toConvert)
		{
			return decimal.Parse(toConvert);
		};

		/// <summary>
		/// Initializes the <see cref="StringParser"/> class.
		/// </summary>
		/// <history>Comment Created:     [MDB]     7/27/2009 </history>
		static StringParser()
		{
			//Setup String to value Converter
			_converterMap.Add(typeof(string), StringConverter);
			_converterMap.Add(typeof(Guid), GuidConverter);
			_converterMap.Add(typeof(Guid?), GuidConverter);
			_converterMap.Add(typeof(int), IntegerConverter);
			_converterMap.Add(typeof(int?), IntegerConverter);
			_converterMap.Add(typeof(DateTime), DateConverter);
			_converterMap.Add(typeof(DateTime?), DateConverter);
			_converterMap.Add(typeof(bool), BoolConverter);
			_converterMap.Add(typeof(bool?), BoolConverter);
			_converterMap.Add(typeof(decimal), DecimalConverter);
			_converterMap.Add(typeof(decimal?), DecimalConverter);
		}

		/// <summary>
		/// Converts the specified value to the given type
		/// </summary>
		/// <typeparam name="T">Type to convert to</typeparam>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		/// <history>Comment Created:     [MDB]     8/21/2009 </history>
		public static T Convert<T>(string value)
		{
			return (T) Convert(typeof(T), value);
		}

		/// <summary>
		/// Converts the specified value into the given type.
		/// </summary>
		/// <param name="retType">Type to convert the value to.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		/// <history>Comment Created:     [MDB]     3/4/2009 </history>
		public static object Convert(System.Type retType, string value)
		{
			if (retType == null)
			{
				throw new ArgumentNullException("retType");
			}
			if (string.IsNullOrWhiteSpace(value))
			{
				throw new ArgumentNullException("value");
			}
			if (retType != typeof(String))
			{
				value = value.Trim();
			}

			return _converterMap[retType](value);
		}

		/// <summary>
		/// Returns true if this class can handle the given type.
		/// </summary>
		/// <param name="typeInQuestion">The type in question.</param>
		/// <returns></returns>
		/// <history>Comment Created:     [MDB]     3/4/2009 </history>
		public static bool HandlesType(System.Type typeInQuestion)
		{
			bool canHandle = false;

			if (typeInQuestion != null)
			{
				canHandle =  _converterMap.ContainsKey(typeInQuestion);
			}

			return canHandle;
		}

		/// <summary>
		/// Determines if two strings are equivalent. This does a type check to determine if the 
		/// underlying type of the string (i.e. string representations of dates or doubles) are equal
		/// as well.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns></returns>
		/// <history>Comment Created:     [MDB]     2/3/2009 </history>
		public static bool AreEqual(string x, string y)
		{
			bool equalStrings = false;

			x = x.Trim();
			y = y.Trim();

			if (string.IsNullOrEmpty(x))
			{
				if (string.IsNullOrEmpty(y))
				{
					//If x is null and y is null, they're equal. 
					equalStrings = true;
				}
				else
				{
					//If x is null and y is not null, not equal
					equalStrings = false;
				}
			}
			else
			{
				if (string.IsNullOrEmpty(y))
				{
					//If x is not null and y is null, not equal
					equalStrings = false;
				}
				else
				{
					if (x.Equals(y))
					{
						//This means the string values are identical. Good enough for us
						equalStrings = true;
					}
					else
					{
						//x and y are both non-null, non-empty strings. Compare their internal values
						//find out what type the strings are, and then compare the types
						System.DateTime xDate = System.DateTime.MinValue;
						bool xIsDate = System.DateTime.TryParse(x, out xDate);

						if (xIsDate)
						{
							System.DateTime yDate = System.DateTime.MinValue;
							if (System.DateTime.TryParse(y, out yDate))
							{
								equalStrings = System.DateTime.Equals(xDate, yDate);
							}
							else
							{
								equalStrings = false;
							}
						}
						else
						{
							//Check if values are Doubles - otherwise these values are not equal
							double xDouble = double.NaN;
							bool xIsDouble = double.TryParse(x, out xDouble);

							if (xIsDouble)
							{
								double yDouble = double.NaN;
								if (double.TryParse(y, out yDouble))
								{
									equalStrings = double.Equals(xDouble, yDouble);
								}
								else
								{
									equalStrings = false;
								}
							}
							else
							{
								equalStrings = false;
							}
						}

					}
				}
			}

			return equalStrings;
		} 
	}
}
