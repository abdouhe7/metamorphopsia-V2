using System;

public class UniqueIdentifierGenerator
{
    public static string GetUniqueIdentifier()
    {
        // Get the current date and time
        DateTime now = DateTime.Now;

        // Format the date and time to create a unique identifier
        string uniqueID = now.ToString("yyyyMMddHHmmssfff");

        return uniqueID;
    }
}
