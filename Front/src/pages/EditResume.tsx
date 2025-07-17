import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import Navbar from "@/components/shared/Navbar";
import ResumeForm from "@/components/resume/ResumeForm";
import { getUser } from "@/utils/localStorage";
import curriculumService from "@/services/curriculumService";
import { useToast } from "@/components/ui/use-toast";
import { Loader2 } from "lucide-react";

const EditResume = () => {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const { toast } = useToast();
  const [curriculumData, setCurriculumData] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  
  useEffect(() => {
    const user = getUser();
    if (!user) {
      navigate("/login");
      return;
    }

    if (!id) {
      toast({
        variant: "destructive",
        title: "Erro",
        description: "ID do currículo não encontrado",
      });
      navigate("/dashboard");
      return;
    }

    loadCurriculumData();
  }, [navigate, id]);

  const loadCurriculumData = async () => {
    try {
      setIsLoading(true);
      const data = await curriculumService.getById(id!);
      setCurriculumData(data);
    } catch (error: any) {
      toast({
        variant: "destructive",
        title: "Erro ao carregar currículo",
        description: error.message || "Não foi possível carregar os dados do currículo",
      });
      navigate("/dashboard");
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return (
      <div className="min-h-screen flex flex-col bg-gray-50">
        <Navbar />
        <div className="container mx-auto px-4 py-8 flex-1 flex justify-center items-center">
          <div className="flex items-center gap-2">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
            <span className="text-gray-600">Carregando dados do currículo...</span>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      <Navbar />
      <div className="container mx-auto px-4 py-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold">Editar Currículo</h1>
          <p className="text-gray-600 mt-2">
            Atualize as informações do seu currículo profissional.
          </p>
        </div>
        {curriculumData && (
          <ResumeForm 
            mode="edit" 
            curriculumId={id!}
            initialData={curriculumData} 
          />
        )}
      </div>
    </div>
  );
};

export default EditResume;
