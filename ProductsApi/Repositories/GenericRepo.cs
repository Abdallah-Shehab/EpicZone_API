using ProductsApi.Models;
using ProductsApi.UnitOFWork;

namespace ProductsApi.Repositories
{
    public class GenericRepo<TEntity> where TEntity : class
    {
        EpicContext context;
        public GenericRepo(EpicContext context)
        {
            this.context = context;
        }
        public List<TEntity> getAll()
        {
            return context.Set<TEntity>().ToList();
        }
        public List<Product> getAllByCategory(string category)
        {
            if (category == null)
            {
                return context.Products.ToList();

            }
            else
            {

                return context.Products.Where(p => p.Category.Contains(category)).ToList();
            }

        }
        public TEntity getByID(int id)
        {
            return context.Set<TEntity>().Find(id);
        }

        public void Add(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
        }

        public void Update(TEntity entity)
        {
            context.Set<TEntity>().Update(entity);
        }

        public void Delete(int id)
        {
            TEntity entity = getByID(id);
            context.Set<TEntity>().Remove(entity);
        }


        public void RepoSave()
        {
            context.SaveChanges();
        }


    }
}
