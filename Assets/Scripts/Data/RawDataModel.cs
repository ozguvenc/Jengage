using System.Collections.Generic;

[System.Serializable]
public class RawDataModel
{
    public int id;
    public string subject;
    public string grade;
    public int mastery;
    public string domainid;
    public string domain;
    public string cluster;
    public string standardid;
    public string standarddescription;
}

[System.Serializable]
public class RawDataModelList
{
    public List<RawDataModel> items;
}
