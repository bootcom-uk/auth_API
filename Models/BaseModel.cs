using System.Text.Json;

namespace Models
{
    public abstract class BaseModel
    {

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}
