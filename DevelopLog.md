# ʵ�����޸ĺ�������ݿⷽ��

���� -> ������������ -> ���������������̨
�������� Enable-Migrations

��ʱ����ᷢ���ڳ���˶��һ���ļ��н�Migrations
��������һ��Configuration.cs�ļ�
������Ȼ���޸ĳ��������ӣ�
public Configuration()
{
AutomaticMigrationsEnabled = true; //������true
ContextKey = "codefirst.DAL.BaseContext";
}

�޸���ɺ�����

�������ݿ�
Update-Database -Force
�������ݿ�
Update-Database -Verbose


--��ȡGuid
string Guid = System.Guid.NewGuid().ToString("D");
xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
