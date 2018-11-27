using System;

[System.Serializable]
public class Category
{
    public int id;
    public string name; 

    public Category(int _id, string _name)
    {
        id = _id;
        name = _name;
    }
	
}
