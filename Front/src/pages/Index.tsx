
import React, { useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import Navbar from "@/components/shared/Navbar";
import authService from "@/services/authService";
import { FileText, Plus, Trash2, Link as LinkIcon } from "lucide-react";

const Index = () => {
  const navigate = useNavigate();
  const user = authService.getCurrentUser();

  useEffect(() => {
    // Se o usuário estiver logado, redirecionar para o dashboard
    if (user) {
      navigate("/dashboard");
    }
  }, [user, navigate]);

  // Se o usuário estiver logado, não renderizar nada (será redirecionado)
  if (user) {
    return null;
  }

  return (
    <div className="min-h-screen flex flex-col">
      <Navbar />

      <main className="flex-1">
        {/* Seção Principal */}
        <section className="bg-gradient-to-b from-white to-blue-50 py-20">
          <div className="container mx-auto px-4 text-center">
            <h1 className="text-4xl md:text-5xl font-bold text-primary mb-6">
              Crie Seu Currículo Profissional em Minutos
            </h1>
            <p className="text-xl text-gray-600 max-w-3xl mx-auto mb-8">
              Construa um currículo que se destaca e compartilhe com empregadores através de um link único. Sem necessidade de download.
            </p>
            <div className="flex flex-col sm:flex-row justify-center gap-4">
              <Link to="/register">
                <Button size="lg" className="text-lg px-8">
                  Começar Agora
                </Button>
              </Link>
              <Link to="/login">
                <Button variant="outline" size="lg" className="text-lg px-8">
                  Entrar
                </Button>
              </Link>
            </div>
          </div>
        </section>

        {/* Seção de Recursos */}
        <section className="py-20">
          <div className="container mx-auto px-4">
            <h2 className="text-3xl font-bold text-center mb-12">
              Por que Escolher o CVFast?
            </h2>
            <div className="grid md:grid-cols-3 gap-8">
              <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-100 text-center">
                <div className="w-16 h-16 bg-primary/10 rounded-full flex items-center justify-center mx-auto mb-4">
                  <FileText className="h-8 w-8 text-primary" />
                </div>
                <h3 className="text-xl font-semibold mb-3">Fácil de Criar</h3>
                <p className="text-gray-600">
                  Nosso construtor de currículos intuitivo torna simples criar um currículo profissional em minutos.
                </p>
              </div>
              <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-100 text-center">
                <div className="w-16 h-16 bg-primary/10 rounded-full flex items-center justify-center mx-auto mb-4">
                  <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="h-8 w-8 text-primary">
                    <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path>
                    <polyline points="7 10 12 15 17 10"></polyline>
                    <line x1="12" y1="15" x2="12" y2="3"></line>
                  </svg>
                </div>
                <h3 className="text-xl font-semibold mb-3">Links Compartilháveis</h3>
                <p className="text-gray-600">
                  Gere links únicos para compartilhar seu currículo diretamente com potenciais empregadores.
                </p>
              </div>
              <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-100 text-center">
                <div className="w-16 h-16 bg-primary/10 rounded-full flex items-center justify-center mx-auto mb-4">
                  <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="h-8 w-8 text-primary">
                    <rect x="2" y="7" width="20" height="14" rx="2" ry="2"></rect>
                    <path d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16"></path>
                  </svg>
                </div>
                <h3 className="text-xl font-semibold mb-3">Templates Profissionais</h3>
                <p className="text-gray-600">
                  Faça seu currículo se destacar com nossos templates desenvolvidos profissionalmente.
                </p>
              </div>
            </div>
          </div>
        </section>

        {/* Seção CTA */}
        <section className="bg-primary text-white py-16">
          <div className="container mx-auto px-4 text-center">
            <h2 className="text-3xl font-bold mb-6">
              Pronto para dar o próximo passo na sua carreira?
            </h2>
            <p className="text-xl mb-8 max-w-2xl mx-auto">
              Crie seu currículo profissional hoje e comece a compartilhá-lo com potenciais empregadores.
            </p>
            <Link to="/register">
              <Button size="lg" variant="secondary" className="text-primary font-semibold">
                Criar Seu Currículo
              </Button>
            </Link>
          </div>
        </section>
      </main>

      <footer className="bg-gray-100 py-8">
        <div className="container mx-auto px-4 text-center">
          <p className="text-gray-600">
            © {new Date().getFullYear()} CVFast. Todos os direitos reservados.
          </p>
        </div>
      </footer>
    </div>
  );
};

export default Index;
