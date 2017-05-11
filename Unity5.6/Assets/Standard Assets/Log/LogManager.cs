using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;


//create by handongji 2015.07.29
public class LogManager : MonoBehaviour 
{
	#region Public Attributes
	public enum LogColor
	{
		White,
		Blue,
		Yellow,
		Red,
		Green
	}
	
	public enum LogForm
	{
		Default,
		Waring,
		Error
	}
	#endregion
	
	#region Private Attributes

	#endregion

	#region Unity Messages
	void Start () 
	{
	
	}

	void Update () 
	{
	
	}
	#endregion

	#region Public Methods
	public static void Log(object messages,bool isWantWriteLogToFile = false,string writeLogFileName =null)
	{
		if (isOpenLog()) 
		{
			if (messages != null) 
			{
				Debug.Log(messages);
				#if UNITY_EDITOR
				if (isWantWriteLogToFile) 
				{
					string filename = null;
					if (writeLogFileName != null) 
					{
						filename = writeLogFileName + ".txt";
					}
					else
					{
						filename = "NoNameLogFile.txt";
					}
					
					CreateLogFile(messages.ToString(),filename);
				}

				#endif
			}
			else
			{
				#if UNITY_EDITOR
				if (isWantWriteLogToFile) 
				{
					Debug.LogError("Please Check Your Params, It's null,File Write Fail");
				}
				else
				{
					Debug.LogError("Please Check Your Params, It's null");
				}		
				#else
				Debug.Log("Please Check Your Params, It's null");
				#endif
			}
		}
	}
	
	public static void Log(object messages,int size)
	{
		if (isOpenLog()) 
		{
			if (messages != null) 
			{
				Debug.Log("<size="+size + ">"+ messages.ToString() +"</size>");
			}
			else
			{
				#if UNITY_EDITOR
				Debug.LogError("<size=" + size +">Please Check Your Params, It's null</size>");
				#else
				Debug.Log("<size=" + size +">Please Check Your Params, It's null</size>");
				#endif
			}
		}
	}
	
	public static void LogWarning(object messages,bool isWantWriteLogToFile = false,string writeLogFileName =null)
	{
		if (isOpenLog()) 
		{
			if (messages != null) 
			{
				Debug.LogWarning(messages);
				#if UNITY_EDITOR
				if (isWantWriteLogToFile) 
				{
					string filename = null;
					if (writeLogFileName != null) 
					{
						filename = writeLogFileName + ".txt";
					}
					else
					{
						filename = "NoNameLogFile.txt";
					}
					
					CreateLogFile(messages.ToString(),filename);
				}
				
				#endif
			}
			else
			{
				#if UNITY_EDITOR
				if (isWantWriteLogToFile) 
				{
					Debug.LogError("Please Check Your Params, It's null,File Write Fail");
				}
				else
				{
					Debug.LogError("Please Check Your Params, It's null");
				}		
				#else
				Debug.Log("Please Check Your Params, It's null");
				#endif
			}
		}
	}
	
	public static void LogError(object messages,bool isWantWriteLogToFile = false,string writeLogFileName =null)
	{
		if (isOpenLog()) 
		{
			if (messages != null) 
			{
				Debug.LogError(messages);
				#if UNITY_EDITOR
				if (isWantWriteLogToFile) 
				{
					string filename = null;
					if (writeLogFileName != null) 
					{
						filename = writeLogFileName + ".txt";
					}
					else
					{
						filename = "NoNameLogFile.txt";
					}
					
					CreateLogFile(messages.ToString(),filename);
				}
				
				#endif
			}
			else
			{
				#if UNITY_EDITOR
				if (isWantWriteLogToFile) 
				{
					Debug.LogError("Please Check Your Params, It's null,File Write Fail");
				}
				else
				{
					Debug.LogError("Please Check Your Params, It's null");
				}		
				#else
				Debug.Log("Please Check Your Params, It's null");
				#endif
			}
		}
	}	
				
	public static void Log(object messages,LogColor color)
	{
		if (isOpenLog()) 
		{
			string colorStr = null;
			
			switch (color) 
			{
				case LogColor.White:
						colorStr = "white";
					break;
					
				case LogColor.Blue:
						colorStr = "blue";
					break;
					
				case LogColor.Yellow:
						colorStr = "yellow";
					break;
					
				case LogColor.Red:
						colorStr = "red";
					break;
					
				case LogColor.Green:
						colorStr = "green";
					break;
				
			}
			
			if (messages != null) 
			{
				Debug.Log("<color="+colorStr+ ">"+ messages.ToString() +"</color>");
			}
			else
			{
				#if UNITY_EDITOR
				Debug.LogError("<color="+colorStr+ ">Please Check Your Params, It's null</color>");
				#else
				Debug.Log("<color="+colorStr+ ">Please Check Your Params, It's null</color>");
				#endif
			}
			
		}
	}
	
	public static void Log(object messages,LogColor color,int size)
	{
		if (isOpenLog()) 
		{
			string colorStr = null;
			
			switch (color) 
			{
			case LogColor.White:
				colorStr = "white";
				break;
				
			case LogColor.Blue:
				colorStr = "blue";
				break;
				
			case LogColor.Yellow:
				colorStr = "yellow";
				break;
				
			case LogColor.Red:
				colorStr = "red";
				break;
				
			case LogColor.Green:
				colorStr = "green";
				break;
				
			}
			
			if (messages != null) 
			{
				Debug.Log("<color="+colorStr+ "><size="+size + ">"+ messages.ToString() +"</size></color>");
			}
			else
			{
				#if UNITY_EDITOR
				Debug.LogError("<color="+colorStr+ "><size=" + size +">Please Check Your Params, It's null</size></color>");
				#else
				Debug.Log("<color="+colorStr+ "><size=" + size +">Please Check Your Params, It's null</size></color>");
				#endif
			}
			
		}
	}
	
	public static void Log(object messages,LogForm form,bool isWantWriteLogToFile = false,string writeLogFileName =null)
	{
		if (isOpenLog()) 
		{
			switch (form) 
			{
				case LogForm.Default:
						if (messages != null) 
						{
							Debug.Log(messages);
						}
						else
						{
							#if UNITY_EDITOR
							Debug.LogError("Please Check Your Params, It's null");
							#else
							Debug.Log("Please Check Your Params, It's null");
							#endif
						}
				break;
				
				case LogForm.Waring:
						if (messages != null) 
						{
							Debug.LogWarning(messages);
							
							#if UNITY_EDITOR
							if (isWantWriteLogToFile) 
							{
								string filename = null;
								if (writeLogFileName != null) 
								{
									filename = writeLogFileName + ".txt";
								}
								else
								{
									filename = "NoNameLogFile.txt";
								}
								
								CreateLogFile(messages.ToString(),filename);
							}
							
							#endif
						}
						else
						{
							#if UNITY_EDITOR
							if (isWantWriteLogToFile) 
							{
								Debug.LogError("Please Check Your Params, It's null,File Write Fail");
							}
							else
							{
								Debug.LogError("Please Check Your Params, It's null");
							}		
							#else
							Debug.Log("Please Check Your Params, It's null");
							#endif
						}
						
				break;
				
				case LogForm.Error:
						if (messages != null) 
						{
							
							Debug.LogError(messages);
					
							#if UNITY_EDITOR
							if (isWantWriteLogToFile) 
							{
								string filename = null;
								if (writeLogFileName != null) 
								{
									filename = writeLogFileName + ".txt";
								}
								else
								{
									filename = "NoNameLogFile.txt";
								}
								
								CreateLogFile(messages.ToString(),filename);
							}
							#endif
						}
						else
						{
							#if UNITY_EDITOR
					
							if (isWantWriteLogToFile) 
							{
								Debug.LogError("Please Check Your Params, It's null,File Write Fail");
							}
							else
							{
								Debug.LogError("Please Check Your Params, It's null");
							}		
							#else
							Debug.Log("Please Check Your Params, It's null");
							#endif
						}
				break;
			}
		}
	}

	public static void Log(object messages,LogForm form,int size)
	{
		if (isOpenLog()) 
		{
			switch (form) 
			{
				case LogForm.Default:
						if (messages != null) 
						{
							Debug.Log("<size="+size + ">"+ messages.ToString() +"</size>");
						}
						else
						{
							#if UNITY_EDITOR
							Debug.LogError("<size=" + size +">Please Check Your Params, It's null</size>");
							#else
							Debug.Log("<size=" + size +">Please Check Your Params, It's null</size>");
							#endif
						}
					break;
					
				case LogForm.Waring:
						if (messages != null) 
						{
							Debug.LogWarning("<size="+size + ">"+ messages.ToString() +"</size>");
						}
						else
						{
							#if UNITY_EDITOR
							Debug.LogError("<size=" + size +">Please Check Your Params, It's null</size>");
							#else
							Debug.Log("<size=" + size +">Please Check Your Params, It's null</size>");
							#endif
						}
					break;
					
				case LogForm.Error:
						if (messages != null) 
						{
							Debug.LogError("<size="+size + ">"+ messages.ToString() +"</size>");
						}
						else
						{
							#if UNITY_EDITOR
							Debug.LogError("<size=" + size +">Please Check Your Params, It's null</size>");
							#else
							Debug.Log("<size=" + size +">Please Check Your Params, It's null</size>");
							#endif
						}
					break;
				}
		}
	}
	#endregion

	#region Private Methods
	private static bool isOpenLog()
	{
		
        return true;
	}
	
	/*
		fileName：文件的名称
		writeInfo：写入的内容
	*/
	private static void CreateLogFile(string writeInfo, string fileName)
	{
		string path = Application.dataPath+"/LogFiles/";
		//创建文件流
		StreamWriter streamWriter;
		FileInfo fInfo = new FileInfo(path+fileName);
		//判断文件流是否存在
		if(!fInfo.Exists)
		{
			//如果没有就创建
			streamWriter = fInfo.CreateText();
		}
		else
		{  
			//如果有就打开
			streamWriter = fInfo.AppendText();
		}
		//将要写入的信息以行写入
		streamWriter.WriteLine(writeInfo);
		//关闭和销毁流
		streamWriter.Flush();
		streamWriter.Close();
		streamWriter.Dispose();
	} 
	#endregion
}
