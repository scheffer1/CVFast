
import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Navbar from "@/components/shared/Navbar";
import ResumePreview from "@/components/resume/ResumePreview";
import { getResumeById, getResumeByShareableLink, getUser } from "@/utils/localStorage";
import { Resume } from "@/types";

const PreviewResume = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [resume, setResume] = useState<Resume | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isOwner, setIsOwner] = useState(false);

  useEffect(() => {
    if (!id) {
      navigate("/dashboard");
      return;
    }

    // Primeiro tenta encontrar pelo ID (para visualização do proprietário)
    let foundResume = getResumeById(id);
    
    // Se não encontrado pelo ID, tenta pelo link compartilhável (para visualização da empresa)
    if (!foundResume) {
      const currentUrl = window.location.href;
      foundResume = getResumeByShareableLink(currentUrl);
    }

    if (foundResume) {
      setResume(foundResume);
      
      // Verifica se o usuário atual é o proprietário
      const currentUser = getUser();
      if (currentUser && currentUser.id === foundResume.userId) {
        setIsOwner(true);
      }
    } else {
      navigate("/not-found");
    }
    
    setIsLoading(false);
  }, [id, navigate]);

  if (isLoading) {
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
        <ResumePreview resume={resume} />
      </div>
    </div>
  );
};

export default PreviewResume;
