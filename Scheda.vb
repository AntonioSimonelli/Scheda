Imports System
Imports System.IO.Ports
Imports System.Threading
Public Class EmettiScheda
	Const EOL As Char = Microsoft.VisualBasic.Chr(&H06)
	Const ESC As Char = Microsoft.VisualBasic.Chr(&H1B)
	Shared _port As SerialPort
	Shared _continue As Boolean
	Shared _thread As Thread

	Shared Sub Init
		_port.ReadTimeout = 500
		Try
			_port.NewLine = EOL
			_port.Open
			_port.Write(ESC & "M1")
			_port.Write(ESC & "M1")
			_port.ReadExisting
			_port.Write(ESC & "E")
		Catch e As Exception
			Dim sw As New System.IO.StreamWriter("Error.log",True)
			sw.Write(Now & e.ToString & vbCrLf)
			sw.Close()
		End Try
	End Sub
	Shared Sub ReadSerial
		Dim info As String = ""
		Monitor.Enter(_continue)
		Try
			Try
				If _port.BytesToRead > 1 Then
				info = _port.ReadLine
				_port.Write(ESC & "E")
				Dim sw As New System.IO.StreamWriter("Data.txt",True)
				sw.Write(info & vbCrLf)
				sw.Close()
			End If
			Catch  e As Exception
				Dim sw As New System.IO.StreamWriter("Error.log",True)
				sw.Write(Now & e.ToString & vbCrLf)
				sw.Close()
			End Try
		Finally
			Monitor.Exit(_continue)
		End Try
	End Sub
	Protected Overrides Sub Finalize()
		_thread.Join
		MyBase.Finalize()
	End Sub

	Public Shared Sub Main
		_thread = New Thread(AddressOf ReadSerial)
		_continue = True
		_port = New SerialPort
		Init
		_thread.Start
		While _continue
			Thread.Sleep(50)
		End While
	End Sub
End Class