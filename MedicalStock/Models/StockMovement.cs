using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalStock.Models
{
    public class StockMovement
    {
        public int Id { get; private set; }
        public int BatchId { get; set; }
        public Batch? Batch { get; set; }
        public int Quantity { get; set; }
        public MovementType Type { get; set; }
        public DateTime MovementDate { get; set; }

        public StockMovement() { }

        public StockMovement(Batch batch, int quantity, MovementType type, DateTime? movementDate)
        {
            Batch = batch;
            Quantity = quantity;
            Type = type;
            if (!movementDate.HasValue || movementDate == DateTime.MinValue)
            {
                MovementDate = DateTime.Now;
            }
            else
                MovementDate = movementDate.Value;
        }

        public override string ToString()
        {
            return $"Movement ID: {Id}, Batch ID: {BatchId}, Quantity: {Quantity}, " +
                $"Movement Type: {Type}, Movement Date: {MovementDate.ToString("yyyy-MM-dd")};";
        }
    }
}
