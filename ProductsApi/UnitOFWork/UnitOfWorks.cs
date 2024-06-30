using ProductsApi.Models;
using ProductsApi.Repositories;

namespace ProductsApi.UnitOFWork
{
    public class UnitOfWorks
    {
        EpicContext context;

        GenericRepo<Product> Repo;
        public UnitOfWorks(EpicContext context)
        {
            this.context = context;
        }


        public GenericRepo<Product> ProductRepo
        {
            get
            {
                if (Repo == null)
                {
                    Repo = new GenericRepo<Product>(context);
                }
                return Repo;
            }
        }

        public void UnitSave()
        {
            context.SaveChanges();
        }
    }
}
