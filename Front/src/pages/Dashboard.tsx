
import React, { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Switch } from "@/components/ui/switch";
import { Label } from "@/components/ui/label";
import Navbar from "@/components/shared/Navbar";
import curriculumService from "@/services/curriculumService";
import authService from "@/services/authService";
import { FileText, Plus, Trash2, Link as LinkIcon, Loader2, Eye, EyeOff } from "lucide-react";
import { useToast } from "@/components/ui/use-toast";

// Interface para currículos do backend
interface BackendCurriculum {
  id: string;
  userId: string;
  title: string;
  summary?: string;
  status: string;
  createdAt: string;
  updatedAt: string;
  shortLinks?: Array<{
    id: string;
    curriculumId: string;
    hash: string;
    accessUrl: string;
    isRevoked: boolean;
    createdAt: string;
    revokedAt?: string;
  }>;
}

const Dashboard = () => {
  const { toast } = useToast();
  const navigate = useNavigate();
  const [curriculums, setCurriculums] = useState<BackendCurriculum[]>([]);
  const [user, setUser] = useState(authService.getCurrentUser());
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    if (!user) {
      navigate("/login");
      return;
    }

    loadCurriculums();
  }, [user, navigate]);

  const loadCurriculums = async () => {
    try {
      setIsLoading(true);
      const userCurriculums = await curriculumService.getUserCurriculums();
      setCurriculums(userCurriculums);
    } catch (error: any) {
      toast({
        variant: "destructive",
        title: "Erro ao carregar currículos",
        description: error.message || "Não foi possível carregar seus currículos",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteCurriculum = async (id: string) => {
    if (window.confirm("Tem certeza que deseja excluir este currículo?")) {
      try {
        await curriculumService.delete(id);
        setCurriculums(curriculums.filter(curriculum => curriculum.id !== id));
        toast({
          title: "Currículo excluído",
          description: "Seu currículo foi permanentemente excluído",
        });
      } catch (error: any) {
        toast({
          variant: "destructive",
          title: "Erro ao excluir currículo",
          description: error.message || "Não foi possível excluir o currículo",
        });
      }
    }
  };

  const copyLink = (link: string) => {
    navigator.clipboard.writeText(link);
    toast({
      title: "Link copiado",
      description: "O link do currículo foi copiado para a área de transferência",
    });
  };

  const handleVisibilityChange = async (curriculumId: string, isPublic: boolean) => {
    try {
      await curriculumService.updateVisibility(curriculumId, isPublic);

      // Atualizar o estado local
      setCurriculums(curriculums.map(curriculum =>
        curriculum.id === curriculumId
          ? { ...curriculum, status: isPublic ? 'Active' : 'Hidden' }
          : curriculum
      ));

      toast({
        title: "Visibilidade alterada",
        description: `Currículo agora está ${isPublic ? 'público' : 'privado'}`,
      });
    } catch (error: any) {
      toast({
        variant: "destructive",
        title: "Erro ao alterar visibilidade",
        description: error.message || "Não foi possível alterar a visibilidade do currículo",
      });
    }
  };

  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      <Navbar />
      <div className="container mx-auto px-4 py-8 flex-1">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold">Meus Currículos</h1>
          <Link to="/create-resume">
            <Button className="flex items-center gap-2">
              <Plus className="h-4 w-4" /> Criar Novo Currículo
            </Button>
          </Link>
        </div>

        {isLoading ? (
          <div className="flex justify-center items-center py-12">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
            <span className="ml-2 text-gray-600">Carregando currículos...</span>
          </div>
        ) : curriculums.length === 0 ? (
          <Card className="bg-white border-dashed border-2 border-gray-300">
            <CardContent className="flex flex-col items-center justify-center py-12">
              <FileText className="h-16 w-16 text-gray-400 mb-4" />
              <h3 className="text-xl font-semibold text-gray-700 mb-2">Nenhum currículo ainda</h3>
              <p className="text-gray-500 text-center max-w-md mb-6">
                Crie seu primeiro currículo para compartilhar com potenciais empregadores
              </p>
              <Link to="/create-resume">
                <Button>Criar Currículo</Button>
              </Link>
            </CardContent>
          </Card>
        ) : (
          <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
            {curriculums.map((curriculum) => (
              <Card key={curriculum.id} className="bg-white">
                <CardHeader>
                  <CardTitle className="flex justify-between items-start">
                    <span className="truncate">{curriculum.title}</span>
                    <Button
                      variant="ghost"
                      size="icon"
                      className="text-red-500 hover:text-red-700 hover:bg-red-50"
                      onClick={() => handleDeleteCurriculum(curriculum.id)}
                    >
                      <Trash2 className="h-5 w-5" />
                    </Button>
                  </CardTitle>
                  <CardDescription>
                    {curriculum.summary || "Sem descrição"}
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="space-y-3">
                    <div>
                      <h4 className="text-sm font-medium text-gray-500">Status</h4>
                      <span className={`text-xs px-2 py-1 rounded ${
                        curriculum.status === 'Active' ? 'bg-green-100 text-green-800' :
                        curriculum.status === 'Draft' ? 'bg-yellow-100 text-yellow-800' :
                        curriculum.status === 'Hidden' ? 'bg-gray-100 text-gray-800' :
                        'bg-red-100 text-red-800'
                      }`}>
                        {curriculum.status === 'Active' ? 'Ativo' :
                         curriculum.status === 'Draft' ? 'Rascunho' :
                         curriculum.status === 'Hidden' ? 'Oculto' :
                         curriculum.status === 'Archived' ? 'Arquivado' : curriculum.status}
                      </span>
                    </div>
                    <div>
                      <h4 className="text-sm font-medium text-gray-500">Link Curto</h4>
                      <p className="text-sm truncate">
                        {curriculum.shortLinks && curriculum.shortLinks.length > 0 && !curriculum.shortLinks[0].isRevoked
                          ? `cvfast.com/${curriculum.shortLinks[0].hash}`
                          : "Nenhum link ativo"}
                      </p>
                    </div>
                    <div>
                      <h4 className="text-sm font-medium text-gray-500">Última Atualização</h4>
                      <p className="text-sm">
                        {new Date(curriculum.updatedAt).toLocaleDateString('pt-BR')}
                      </p>
                    </div>
                    <div className="flex items-center justify-between">
                      <div className="flex items-center space-x-2">
                        <EyeOff className="h-4 w-4 text-gray-500" />
                        <Label htmlFor={`visibility-${curriculum.id}`} className="text-sm font-medium text-gray-500">
                          Visibilidade
                        </Label>
                        <Eye className="h-4 w-4 text-gray-500" />
                      </div>
                      <Switch
                        id={`visibility-${curriculum.id}`}
                        checked={curriculum.status === 'Active'}
                        onCheckedChange={(checked) => handleVisibilityChange(curriculum.id, checked)}
                      />
                    </div>
                  </div>
                </CardContent>
                <CardFooter className="flex flex-col space-y-3">
                  <Link to={`/resume/${curriculum.id}`} className="w-full">
                    <Button variant="default" className="w-full">
                      Visualizar Currículo
                    </Button>
                  </Link>
                  {curriculum.shortLinks.length > 0 && !curriculum.shortLinks[0].isRevoked && (
                    <Button
                      variant="outline"
                      className="w-full flex items-center gap-2"
                      onClick={() => copyLink(`${window.location.origin}/s/${curriculum.shortLinks[0].hash}`)}
                    >
                      <LinkIcon className="h-4 w-4" /> Copiar Link Compartilhável
                    </Button>
                  )}
                </CardFooter>
              </Card>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default Dashboard;
