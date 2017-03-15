using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rvt2Excel.Parallel
{
    sealed class SolidTaskBucket : TaskBucket<IList<Element>>
    {
        private FilteredElementCollector collector;
        private ElementIntersectsSolidFilter solidFilter;
        public Element Self { get; private set; }

        public SolidTaskBucket(Element element)
        {
            GeometryElement geomElement = element.get_Geometry(new Options { ComputeReferences = true });
            foreach (var geomObj in geomElement)
            {
                Solid solid = geomObj as Solid;
                if (solid != null && solid.Volume != 0)
                {
                    solidFilter = new ElementIntersectsSolidFilter(solid);
                    break;
                }
            }
            Self = element;
        }

        public void SetElementCollector(Document doc)
        {
            collector = new FilteredElementCollector(doc);
        }

        public void SetElementCollector(Document doc, ICollection<ElementId> elementIds)
        {
            collector = new FilteredElementCollector(doc, elementIds);
        }

        public override void ExecuteAsync()
        {
            if (collector != null && solidFilter != null)
            {
                TaskAsync = Task.Run(() => collector.WherePasses(solidFilter).ToElements());
            }
            else
            {
                TaskAsync = Task.FromResult(new List<Element>() as IList<Element>);
            }
        }
    }

    static class SolidTaskUtil
    {
        public static IEnumerable<IList<Element>> ParallelSolidFilter(Document doc, IEnumerable<Element> solidElements)
        {
            SolidTaskBucket[] tasks = GetSolidTaskBuckets(doc, solidElements);
            foreach (var task in tasks)
            {
                task.SetElementCollector(doc);
                task.ExecuteAsync();
            }
            return ConcatResult(tasks);
        }

        public static IEnumerable<IList<Element>> ParallelSolidFilter(Document doc, IEnumerable<Element> solidElements, ICollection<ElementId> filterElements)
        {
            SolidTaskBucket[] tasks = GetSolidTaskBuckets(doc, solidElements);
            foreach (var task in tasks)
            {
                task.SetElementCollector(doc, filterElements);
                task.ExecuteAsync();
            }
            return ConcatResult(tasks);
        }

        private static SolidTaskBucket[] GetSolidTaskBuckets(Document doc, IEnumerable<Element> solidElements)
        {
            SolidTaskBucket[] tasks = new SolidTaskBucket[solidElements.Count()];
            int index = 0;
            foreach (var elem in solidElements)
            {
                tasks[index++] = new SolidTaskBucket(elem);
            }
            return tasks;
        }

        private static IEnumerable<IList<Element>> ConcatResult(SolidTaskBucket[] tasks)
        {
            TaskBucket.WaitAll(tasks);

            foreach (var item in tasks)
            {
                IList<Element> result = item.TaskAsync.Result;
                yield return result.Where(elem => elem.Id.IntegerValue != item.Self.Id.IntegerValue).ToList();
            }
        }
    }
}
