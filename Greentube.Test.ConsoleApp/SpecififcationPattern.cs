using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greentube.Test.ConsoleApp
{
    public class ExpressionSpecification<T> : CompositeSpecification<T>
    {
        private Func<T, bool> expression;
        public ExpressionSpecification(Func<T, bool> expression)
        {
            if (expression == null)
                throw new ArgumentNullException();
            else
                this.expression = expression;
        }

        public override bool IsSatisfiedBy(T o)
        {
            return this.expression(o);
        }
    }  

    public abstract class CompositeSpecification<T> : ISpecification<T>
    {
        public abstract bool IsSatisfiedBy(T o);

        public ISpecification<T> And(ISpecification<T> specification)
        {
            return new AndSpecification<T>(this, specification);
        }
        public ISpecification<T> Or(ISpecification<T> specification)
        {
            return new OrSpecification<T>(this, specification);
        }
        public ISpecification<T> Not(ISpecification<T> specification)
        {
            return new NotSpecification<T>(specification);
        }
    }


    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T o);
        ISpecification<T> And(ISpecification<T> specification);
        ISpecification<T> Or(ISpecification<T> specification);
        ISpecification<T> Not(ISpecification<T> specification);
    }

    public class AndSpecification<T> : CompositeSpecification<T>    
    {
        readonly ISpecification<T> _leftSpecification;
        readonly ISpecification<T> _rightSpecification;

        public AndSpecification(ISpecification<T> left, ISpecification<T> right)  {
            this._leftSpecification = left;
            this._rightSpecification = right;
        }

        public override bool IsSatisfiedBy(T o)   {
            return this._leftSpecification.IsSatisfiedBy(o) 
                && this._rightSpecification.IsSatisfiedBy(o);
        }
    }

    public class OrSpecification<T> : CompositeSpecification<T>
    {
        readonly ISpecification<T> _leftSpecification;
        readonly ISpecification<T> _rightSpecification;

        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            this._leftSpecification = left;
            this._rightSpecification = right;
        }

        public override bool IsSatisfiedBy(T o)
        {
            return this._leftSpecification.IsSatisfiedBy(o)
                || this._rightSpecification.IsSatisfiedBy(o);
        }
    }

    public class NotSpecification<T> : CompositeSpecification<T>
    {
        readonly ISpecification<T> _specification;

        public NotSpecification(ISpecification<T> specification)
        {
            this._specification = specification;
        }

        public override bool IsSatisfiedBy(T o)
        {
            return !this._specification.IsSatisfiedBy(o);
        }
    }
}
