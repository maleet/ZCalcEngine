using System;
using System.Collections.Generic;
using System.Linq;

namespace Zirpl.CalcEngine.Tests
{
    public class TestPerson
    {
	    public decimal m => 1;
	    public decimal h => 1/60;
        public TestPerson()
        {
            Children = new List<TestPerson>();
        }
        public string Name { get; set; }
        public bool Male { get; set; }
        public DateTime? Birth { get; set; }

	    public Address Address { get; set; }
	    public TestPerson Parent { get; set; }
        public List<TestPerson> Children { get; set; }

	    public int? Age => Birth == null ? (int?) null : DateTime.Today.Year - Birth.Value.Year;
	    public double? Weight => Age / 0.1;
	    public decimal? Salary => Age * 55.1M;

	    public Dictionary<string, TestPerson> ChildrenDct
	    {
		    get
		    {
			    return Children.ToDictionary(person => person.Name, person => person);
			}
	    }
	    
	    public Dictionary<string, TestPerson> ChildrenIdDct
		{
			get
			{
				return Children.ToDictionary(person => person.Id.ToString().ToUpperInvariant(), person => person);
			}
		}
	    
	    public Dictionary<string, object> ChildrenAgeDct
	    {
		    get
		    {
			    return Children.ToDictionary(person => person.Name, person => person.Age as object);
		    }
	    }
	    
	    public Dictionary<string, object> ChildrenWeightDct
	    {
		    get
		    {
			    return Children.ToDictionary(person => person.Name, person => person.Weight as object);
		    }
	    }
	    
	    public Dictionary<string, object> ChildrenSalaryDct
	    {
		    get
		    {
			    return Children.ToDictionary(person => person.Name, person => person.Salary as object);
		    }
	    }

	    public static TestPerson CreateTestPerson()
        {
            var p = new TestPerson();
            p.Name = "Test Person";
			p.Code = "Code";
			p.Nullable = null;
			p.Id = Guid.Parse("96C5888C-6C75-43DD-A372-2A3398DAE038");
			p.Birth = DateTime.Today.AddYears(-30);
	        p.Number = 55;
            p.Male = true;
			for (int i = 0; i < 5; i++)
			{
				var c = new TestPerson();
				c.Name = "Test Child " + i.ToString();
				c.Id = Guid.Parse(i + "6C5888C-6C75-43DD-A372-2A3398DAE038");
				c.Birth = DateTime.Today.AddYears(-i);
				c.Male = i % 2 == 0;
				c.Number = i;
				p.Children.Add(c);
			}
			return p;
        }

	    public decimal? Nullable { get; set; }

	    public int Number { get; set; }

	    public string Code { get; set; }

	    public Guid Id { get; set; }
    }

	public class Address
	{
		public string Street { get; set; }
	}
}
