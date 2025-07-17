
import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Navbar from "@/components/shared/Navbar";
import ResumePreview from "@/components/resume/ResumePreview";
import curriculumService from "@/services/curriculumService";
import authService from "@/services/authService";
import { Resume } from "@/types";
import { Loader2 } from "lucide-react";
import { useToast } from "@/components/ui/use-toast";

const PreviewResume = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { toast } = useToast();
  const [resume, setResume] = useState<Resume | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isOwner, setIsOwner] = useState(false);

  useEffect(() => {
    if (!id) {
      navigate("/dashboard");
      return;
    }

    loadCurriculum();
  }, [id, navigate]);

  const loadCurriculum = async () => {
    try {
      setIsLoading(true);

      // Buscar o currículo pela API
      const curriculum = await curriculumService.getById(id!);

      // Extrair informações de contato
      const emailContact = curriculum.contacts?.find(c => c.type === 'Email');
      const phoneContact = curriculum.contacts?.find(c => c.type === 'Phone');
      const linkedinContact = curriculum.contacts?.find(c => c.type === 'LinkedIn');
      const githubContact = curriculum.contacts?.find(c => c.type === 'GitHub');
      const websiteContact = curriculum.contacts?.find(c => c.type === 'Website');

      // Extrair endereço principal
      const primaryAddress = curriculum.addresses?.find(a => a.type === 'Current') || curriculum.addresses?.[0];
      const addressString = primaryAddress ? primaryAddress.street : '';

      // Converter os dados do backend para o formato do frontend
      const resumeData: Resume = {
        id: curriculum.id,
        userId: curriculum.userId,
        personalInfo: {
          fullName: curriculum.title,
          title: curriculum.summary || '',
          email: emailContact?.value || '',
          phone: phoneContact?.value || '',
          address: addressString,
          summary: curriculum.summary || '',
          linkedin: linkedinContact?.value || '',
          github: githubContact?.value || '',
          website: websiteContact?.value || ''
        },
        education: curriculum.educations?.map(edu => ({
          id: edu.id,
          institution: edu.institution,
          degree: edu.degree,
          fieldOfStudy: edu.fieldOfStudy,
          startDate: edu.startDate,
          endDate: edu.endDate,
          current: !edu.endDate,
          description: edu.description || ''
        })) || [],
        experience: curriculum.experiences?.map(exp => ({
          id: exp.id,
          company: exp.companyName,
          position: exp.role,
          location: exp.location || '',
          startDate: exp.startDate,
          endDate: exp.endDate,
          current: !exp.endDate,
          description: exp.description || ''
        })) || [],
        skills: curriculum.skills?.map(skill => skill.techName) || [],
        languages: curriculum.languages?.map(lang => ({
          language: lang.languageName,
          proficiency: lang.proficiency
        })) || [],
        createdAt: curriculum.createdAt,
        updatedAt: curriculum.updatedAt,
        shareableLink: curriculum.shortLinks?.[0]?.hash ?
          `${window.location.origin}/s/${curriculum.shortLinks[0].hash}` : ''
      };

      setResume(resumeData);

      // Verifica se o usuário atual é o proprietário
      const currentUser = authService.getCurrentUser();
      if (currentUser && currentUser.id === curriculum.userId) {
        setIsOwner(true);
      }
    } catch (error: any) {
      toast({
        variant: "destructive",
        title: "Erro ao carregar currículo",
        description: error.message || "Não foi possível carregar o currículo",
      });
      navigate("/not-found");
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return (
      <div className="min-h-screen flex flex-col">
        <Navbar />
        <div className="flex-1 flex items-center justify-center">
          <Loader2 className="h-8 w-8 animate-spin text-primary" />
          <span className="ml-2 text-gray-600">Carregando currículo...</span>
        </div>
      </div>
    );
  }

  if (!resume) {
    return (
      <div className="min-h-screen flex flex-col">
        <Navbar />
        <div className="flex-1 flex items-center justify-center">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto"></div>
            <p className="mt-4 text-gray-600">Carregando currículo...</p>
          </div>
        </div>
      </div>
    );
  }

  if (!resume) {
    return null;
  }

  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      <Navbar />
      <div className="container mx-auto px-4 py-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold">
            {isOwner ? "Seu Currículo" : `Currículo de ${resume.personalInfo.fullName}`}
          </h1>
          {!isOwner && (
            <p className="text-gray-600 mt-2">
              Este currículo foi compartilhado com você por {resume.personalInfo.fullName}.
            </p>
          )}
        </div>
        <ResumePreview resume={resume} isOwner={isOwner} />
      </div>
    </div>
  );
};

export default PreviewResume;
