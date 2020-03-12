namespace DocumentManagment.DataAccess.Models
{
    public class OrderCriteria
    {
        public OrderCriteria(string field, bool isDesc)
        {
            this.Field = field;
            this.IsDesc = isDesc;
        }

        public string Field { get; set; }

        public bool IsDesc { get; set; }
    }
}
