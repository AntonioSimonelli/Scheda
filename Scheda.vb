Imports System
Imports System.IO.Ports
Imports System.Threading
Imports System.Windows.Forms
Public Class Scheda
	Const EOL As Char = Microsoft.VisualBasic.Chr(&H06)
	Const ESC As Char = Microsoft.VisualBasic.Chr(&H1B)
	Protected moPort As SerialPort

	Public Sub New
		moPort = New SerialPort("COM1",9600)
		moPort.ReadTimeout = 100
		AddHandler moPort.DataReceived, AddressOf DataReceived

		Try
			moPort.NewLine = EOL
			moPort.Open
			moPort.Write(ESC & "M1")
			moPort.ReadExisting
			moPort.Write(ESC & "E")
		Catch
		End Try
	End Sub
	Private Sub DataReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
		Try
			If moPort.BytesToRead > 1 Then
				ReadSerial
			End If
		Catch
		End Try
	End Sub
	Private Sub ReadSerial
		Dim info As String = ""
		Monitor.Enter(Me)
		Try
			Try
				info = moPort.ReadLine
				Dim sw As New System.IO.StreamWriter("Data.txt",True)
				sw.Write(info & vbCrLf)
				sw.Close()
			Catch e As Exception
				Dim sw As New System.IO.StreamWriter("Error.log",True)
				sw.Write(e.ToString & vbCrLf)
				sw.Close()
			End Try
			moPort.Write(ESC & "E")
		Catch ex As Exception
			Dim sw As New System.IO.StreamWriter("Error.log",True)
			sw.Write(ex.ToString & vbCrLf)
			sw.Close()
		Finally
			Monitor.Exit(Me)
		End Try
	End Sub

	Public Shared Sub Main
		Dim card As New Scheda
		While True
			Thread.Sleep(50)
			Application.DoEvents
		End While
	End Sub
End Class