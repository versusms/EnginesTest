# EnginesTest
Test Application for IFMO (web-programming)

## ���������
�������� ������ � ��������� ��� � Visual Studio 2012/2013

## ��������� ����������� � ��
�������� ����������� � ��:
* LocalDB-���������
* ��������� MSSQL-������ (������� SQLEXPRESS)

�������� ������� �����������, ���������������� ��������������� ������ � ����� Models/DataModel.Context.cs :
> : base("name=LocalDataModelContainer")

��� ����������� � LocalDB (������������ ���������� �����������)

> : base("name=RemoteDataModelContainer")

��� ����������� � ���������� MSSQL-������ (��������� ��������� ����������� � ����� Web.config)

## �������� ������
SQL-c����� �������� ������ � ����� Models/DataModel.edmx.sql
��� �� - EnginesTest

## ��������� ���������� ��
���������� ������� � ������� ������. ��� ���������� ������ ������� ��������� ������ � �������� URL:
> http://&lt;hostname&gt;:&lt;port&gt;/init

��� ����������� ������ ����� ���������� � ����������� InitController