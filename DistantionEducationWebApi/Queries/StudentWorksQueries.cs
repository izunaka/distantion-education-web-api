namespace DistantionEducationWebApi.Queries
{
    public class StudentWorksQueries
    {
        public string CreateStudentWork()
        {
            return @"
insert into student_work (id, name, description)
values (@id, @name, @description)";
        }

        public string CreateWorkTask()
        {
            return @"
insert into work_task (id, type, description, student_work_id)
values (@id, @type, @description, @studentWorkId)";
        }

        public string CreateTaskAnswer()
        {
            return @"
insert into public.work_task_answer (id, work_task_id, description, is_right)
values (@id, @workTaskId, @description, @isRight)";
        }

        public string GetAllStudentWorks()
        {
            return @"
select 
    id,
    name,
    description
from student_work";
        }

        public string GetStudentWorkById()
        {
            return @"
select 
    sw.id work_id,
    sw.name work_name,
    sw.description work_description,
    wt.id task_id,
    wt.description task_description,
    tt.id task_type_id,
    tt.code task_type_code,
    tt.description task_type_description,
    tt.rule task_type_rule,
    ta.id answer_id,
    ta.description answer_description,
    ta.is_right is_answer_right
from student_work sw
join work_task wt on wt.student_work_id = sw.id
join task_type tt on tt.id = wt.type
join work_task_answer ta on ta.work_task_id = wt.id
where sw.id = @id";
        }

        public string GetAllTaskTypes()
        {
            return @"
select 
    id,
    code,
    description,
    rule
from task_type";
        }

        public string DeleteWork()
        {
            return @"
delete from student_work where id = @id";
        }
    }
}
