
import React, { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import ResumeForm from "@/components/resume/ResumeForm";
import Navbar from "@/components/shared/Navbar";
import { getUser } from "@/utils/localStorage";

const CreateResume = () => {
  const navigate = useNavigate();
  
  useEffect(() => {
    const user = getUser();
    if (!user) {
      navigate("/login");
    }
  }, [navigate]);

  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      <Navbar />
      <div className="container mx-auto px-4 py-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold">Crie Seu Currículo</h1>
          <p className="text-gray-600 mt-2">
            Preencha o formulário abaixo para criar um currículo profissional que você pode compartilhar com empregadores.
          </p>
        </div>
        <ResumeForm />
      </div>
    </div>
  );
};

export default CreateResume;
