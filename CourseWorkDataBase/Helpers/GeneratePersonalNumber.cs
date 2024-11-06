namespace CourseWorkDataBase.Helpers;

public class GeneratePersonalNumber
{
    static public string GenerateRandomNumber()
    {
        Random random = new Random();
        int length = 6;
        
        var number = string.Empty;

        for (int i = 0; i < length; i++)
        {
            number += random.Next(0, 10).ToString();
        }

        return number;
    }
}