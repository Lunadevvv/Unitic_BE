public static class StringTrimmerExtension
{
    public static void TrimAllString(object obj)
    {
        //lấy các properties là string và có thể get, set
        var props = obj.GetType().GetProperties()
            .Where(p => p.PropertyType == typeof(string) && p.CanRead && p.CanWrite);

        foreach (var prop in props)
        {
            // lấy giá trị của property
            var val = (string?)prop.GetValue(obj);
            if (val != null)
            {
                prop.SetValue(obj, val.Trim());
            }
        }
    }
}
