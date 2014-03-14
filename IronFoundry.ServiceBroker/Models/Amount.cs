namespace IronFoundry.ServiceBroker.Models
{
    public class Amount
    {
        public string Denomination { get; set; }
        public float Value { get; set; }

        public override string ToString()
        {
            return string.Format("Amount [ Denomniation: {0}; Value: {1}; ]", Denomination, Value);
        }
    }
}