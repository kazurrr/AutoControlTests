using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    class Car
    {         
        public String Model
        {
            get { return Model; }
            set { Model = value; }
        }

        public String Brand
        {
            get { return Brand; }
            set { Brand = value; }
        }
        public int VIN
        {
            get { return VIN; }
            set { VIN = value; }
        }

        public Car (String Model, String Brand, int VIN)
        {
            this.Model = Model;
            this.Brand = Brand;
            this.VIN = VIN;
        }

        public Car()
        {

        }
    }
}