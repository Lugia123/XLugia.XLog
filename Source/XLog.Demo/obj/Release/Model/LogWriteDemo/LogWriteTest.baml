   M S B A M L   `   `   `  ����   XLugia.Lib.XLog.Demo:    4XLugia.Lib.XLog.Demo.Model.LogWriteDemo.LogWriteTest   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     .��� -     �Name$  bodyGrid�� � RowDefinitions �� $	��50* ��� $
��139* �.+��� -    ��Name$ messageTextBlock��$
��Left=�$
��Wrap=�$��Bottom=�$��10,0,0,9.126q�� $��1��.+��� -    ��Name$ messageTextBox�� �� TextWrapping$
 Wrap=� �� AcceptsReturn . �� -   $ pageMessageTextBlock��$
��Left=�$
��Wrap=�$��Center=�$��2��$��	350,0,0,0q�� $	��780��$��40��$
��Left=�$	��Top=�$��	10,10,0,0q�.+��� -    ��Name$ addTaskButton��$��开始测试��$
��Left=�$��Center=�$	��100��$��24���� -   $ clearTaskButton��$��停止测试��$
��Left=�$��Center=�$	��100��$��24��$��	126,8,0,8q��� -   $ 
copyButton��$��复制结果��$
��Left=�$��Center=�$	��100��$��24��$��	256,0,0,0q��� -   $ disposeButton��$��强制清理内存��$
��Left=�$��Center=�$	��100��$��24��$��
390,10,0,6q�� $	��258��$��40��$
��Left=�$	��Top=�$��
485,95,0,0q�.+��� -	   $ testLogLengthTextBox��$��Right=�$��23��$
 Wrap=�$��Center=�$	��120��$
��1000���� $
��Left=�$
��Wrap=�$'��!测试日志长度（双字节）��$��Center=�� $	��400��$��40��$
��Left=�$	��Top=�$��	10,50,0,0q�.+�	� -
    	�Name$ singleFileRadioButton��$'��!单日志文件写入性能测试��$
��Left=�$��Center=� d� 	IsChecked$
 TrueY�	� -   $ multiFileRadioButton��$'��!多日志文件写入性能测试��$
��Left=�$��Center=�$��	180,0,0,0q�� $	��200��$��40��$
��Left=�$	��Top=�$��
245,95,0,0q�.+��� -   $ taskNumberTextBox��$��Right=�$��23��$
 Wrap=�$��Center=�$	��120��$
��5000���� $
��Left=�$
��Wrap=�$��	并发数��$��Center=�� $	��200��$��40��$
��Left=�$	��Top=�$��	10,95,0,0q�.+��� -   $ maxTestNumberTextBox��$��Right=�$��23��$
 Wrap=�$��Center=�$	��120��$��3000000���� $
��Left=�$
��Wrap=�$��测试总日志数��$��Center=�� -   $  logReadTestGrid��$��2��