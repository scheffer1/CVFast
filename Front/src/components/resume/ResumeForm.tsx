
import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useFieldArray, useForm } from "react-hook-form";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";
import { useToast } from "@/components/ui/use-toast";
import { Education, Experience, Language, PersonalInfo, Resume } from "@/types";
import completeCurriculumService, {
  CreateCompleteCurriculumData,
  CreateExperienceForCurriculumData,
  CreateEducationForCurriculumData,
  CreateSkillForCurriculumData,
  CreateContactForCurriculumData,
  CreateAddressForCurriculumData
} from "@/services/completeCurriculumService";
import authService from "@/services/authService";
import { formatDateForBackend } from "@/utils/dateUtils";
import { Plus, Trash2, Loader2 } from "lucide-react";

const personalInfoSchema = z.object({
  fullName: z.string().min(2, "Nome completo é obrigatório"),
  email: z.string().email("E-mail válido é obrigatório"),
  phone: z.string().min(6, "Número de telefone é obrigatório"),
  address: z.string().min(5, "Endereço é obrigatório"),
  title: z.string().min(2, "Título profissional é obrigatório"),
  summary: z.string().min(10, "Resumo profissional é obrigatório"),
  linkedin: z.string().url("Deve ser uma URL válida").or(z.string().length(0)).optional(),
  github: z.string().url("Deve ser uma URL válida").or(z.string().length(0)).optional(),
  website: z.string().url("Deve ser uma URL válida").or(z.string().length(0)).optional(),
});

const educationSchema = z.object({
  id: z.string(),
  institution: z.string().min(2, "Nome da instituição é obrigatório"),
  degree: z.string().min(2, "Grau acadêmico é obrigatório"),
  fieldOfStudy: z.string().min(2, "Área de estudo é obrigatória"),
  startDate: z.string().min(4, "Data de início é obrigatória"),
  endDate: z.string().optional(),
  current: z.boolean(),
  description: z.string().optional(),
});

const experienceSchema = z.object({
  id: z.string(),
  company: z.string().min(2, "Nome da empresa é obrigatório"),
  position: z.string().min(2, "Cargo é obrigatório"),
  location: z.string().min(2, "Localização é obrigatória"),
  startDate: z.string().min(4, "Data de início é obrigatória"),
  endDate: z.string().optional(),
  current: z.boolean(),
  description: z.string().min(10, "Descrição do trabalho é obrigatória"),
});

const languageSchema = z.object({
  language: z.string().min(2, "Nome do idioma é obrigatório"),
  proficiency: z.enum(["Iniciante", "Intermediário", "Avançado", "Fluente", "Nativo"]),
});

const formSchema = z.object({
  personalInfo: personalInfoSchema,
  education: z.array(educationSchema),
  experience: z.array(experienceSchema),
  skills: z.array(z.string()).min(1, "Pelo menos uma habilidade é obrigatória"),
  languages: z.array(languageSchema),
});

type FormValues = z.infer<typeof formSchema>;

const ResumeForm = () => {
  const { toast } = useToast();
  const navigate = useNavigate();
  const [skillInput, setSkillInput] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const {
    register,
    control,
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      personalInfo: {
        fullName: "",
        email: "",
        phone: "",
        address: "",
        title: "",
        summary: "",
        linkedin: "",
        github: "",
        website: "",
      },
      education: [
        {
          id: crypto.randomUUID(),
          institution: "",
          degree: "",
          fieldOfStudy: "",
          startDate: "",
          endDate: "",
          current: false,
          description: "",
        },
      ],
      experience: [
        {
          id: crypto.randomUUID(),
          company: "",
          position: "",
          location: "",
          startDate: "",
          endDate: "",
          current: false,
          description: "",
        },
      ],
      skills: [],
      languages: [
        {
          language: "",
          proficiency: "Intermediário",
        },
      ],
    },
  });

  const { fields: educationFields, append: appendEducation, remove: removeEducation } = useFieldArray({
    control,
    name: "education",
  });

  const { fields: experienceFields, append: appendExperience, remove: removeExperience } = useFieldArray({
    control,
    name: "experience",
  });

  const { fields: languageFields, append: appendLanguage, remove: removeLanguage } = useFieldArray({
    control,
    name: "languages",
  });

  const skills = watch("skills");

  const handleAddSkill = () => {
    if (skillInput.trim() !== "" && !skills.includes(skillInput.trim())) {
      setValue("skills", [...skills, skillInput.trim()]);
      setSkillInput("");
    }
  };

  const handleRemoveSkill = (skillToRemove: string) => {
    setValue(
      "skills",
      skills.filter((skill) => skill !== skillToRemove)
    );
  };

  const onSubmit = async (data: FormValues) => {
    setIsLoading(true);
    const user = authService.getCurrentUser();

    if (!user) {
      toast({
        variant: "destructive",
        title: "Erro",
        description: "Você precisa estar logado para criar um currículo",
      });
      navigate("/login");
      setIsLoading(false);
      return;
    }

    try {
      // Preparar dados das experiências
      const experiences: CreateExperienceForCurriculumData[] = data.experience
        .filter(exp => exp.company && exp.position)
        .map(exp => ({
          companyName: exp.company,
          role: exp.position,
          description: exp.description,
          startDate: formatDateForBackend(exp.startDate)!,
          endDate: exp.current ? null : formatDateForBackend(exp.endDate || ''),
          location: exp.location
        }));

      // Preparar dados das educações
      const educations: CreateEducationForCurriculumData[] = data.education
        .filter(edu => edu.institution && edu.degree)
        .map(edu => ({
          institution: edu.institution,
          degree: edu.degree,
          fieldOfStudy: edu.fieldOfStudy,
          startDate: formatDateForBackend(edu.startDate)!,
          endDate: edu.current ? null : formatDateForBackend(edu.endDate || ''),
          description: edu.description
        }));

      // Preparar dados das habilidades
      const skills: CreateSkillForCurriculumData[] = data.skills
        .filter(skill => skill.trim())
        .map(skill => ({
          techName: skill,
          proficiency: 'Intermediate' as const // Valor padrão
        }));

      // Preparar dados dos contatos
      const contacts: CreateContactForCurriculumData[] = [];
      const personalInfo = data.personalInfo;

      if (personalInfo.email) {
        contacts.push({
          type: 'Email',
          value: personalInfo.email,
          isPrimary: true
        });
      }

      if (personalInfo.phone) {
        contacts.push({
          type: 'Phone',
          value: personalInfo.phone,
          isPrimary: false
        });
      }

      if (personalInfo.linkedin) {
        contacts.push({
          type: 'LinkedIn',
          value: personalInfo.linkedin,
          isPrimary: false
        });
      }

      if (personalInfo.github) {
        contacts.push({
          type: 'GitHub',
          value: personalInfo.github,
          isPrimary: false
        });
      }

      if (personalInfo.website) {
        contacts.push({
          type: 'Website',
          value: personalInfo.website,
          isPrimary: false
        });
      }

      // Preparar dados dos endereços (se houver)
      const addresses: CreateAddressForCurriculumData[] = [];
      if (personalInfo.address) {
        // Tentar extrair informações do endereço (simplificado)
        const addressParts = personalInfo.address.split(',').map(part => part.trim());
        if (addressParts.length >= 2) {
          addresses.push({
            street: addressParts[0] || 'Não informado',
            number: 'S/N',
            neighborhood: addressParts[1] || 'Não informado',
            city: addressParts[2] || 'Não informado',
            state: addressParts[3] || 'Não informado',
            country: 'Brasil',
            type: 'Current'
          });
        }
      }

      // Criar currículo completo
      const curriculumData: CreateCompleteCurriculumData = {
        title: data.personalInfo.fullName || "Meu Currículo",
        summary: data.personalInfo.summary,
        status: 'Active',
        experiences,
        educations,
        skills,
        contacts,
        addresses
      };

      const curriculum = await completeCurriculumService.create(curriculumData);

      toast({
        title: "Sucesso",
        description: "Seu currículo foi criado com sucesso",
      });

      navigate(`/resume/${curriculum.id}`);
    } catch (error: any) {
      toast({
        variant: "destructive",
        title: "Erro ao criar currículo",
        description: error.message || "Não foi possível criar o currículo. Tente novamente.",
      });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-8">
      {/* Personal Info Section */}
      <div className="resume-form-section">
        <h3>Informações Pessoais</h3>
        <div className="form-row">
          <div className="space-y-2">
            <Label htmlFor="fullName">Nome Completo</Label>
            <Input
              id="fullName"
              placeholder="João Silva"
              {...register("personalInfo.fullName")}
            />
            {errors.personalInfo?.fullName && (
              <p className="text-sm text-red-500">{errors.personalInfo.fullName.message}</p>
            )}
          </div>
          <div className="space-y-2">
            <Label htmlFor="title">Título Profissional</Label>
            <Input
              id="title"
              placeholder="Engenheiro de Software"
              {...register("personalInfo.title")}
            />
            {errors.personalInfo?.title && (
              <p className="text-sm text-red-500">{errors.personalInfo.title.message}</p>
            )}
          </div>
        </div>

        <div className="form-row">
          <div className="space-y-2">
            <Label htmlFor="email">E-mail</Label>
            <Input
              id="email"
              placeholder="joao.silva@exemplo.com"
              {...register("personalInfo.email")}
            />
            {errors.personalInfo?.email && (
              <p className="text-sm text-red-500">{errors.personalInfo.email.message}</p>
            )}
          </div>
          <div className="space-y-2">
            <Label htmlFor="phone">Telefone</Label>
            <Input
              id="phone"
              placeholder="(11) 98765-4321"
              {...register("personalInfo.phone")}
            />
            {errors.personalInfo?.phone && (
              <p className="text-sm text-red-500">{errors.personalInfo.phone.message}</p>
            )}
          </div>
        </div>

        <div className="space-y-2 mt-4">
          <Label htmlFor="address">Endereço</Label>
          <Input
            id="address"
            placeholder="Rua Principal, 123, Cidade, Estado"
            {...register("personalInfo.address")}
          />
          {errors.personalInfo?.address && (
            <p className="text-sm text-red-500">{errors.personalInfo.address.message}</p>
          )}
        </div>

        <div className="space-y-2 mt-4">
          <Label htmlFor="summary">Resumo Profissional</Label>
          <Textarea
            id="summary"
            placeholder="Descreva brevemente sua experiência profissional e habilidades"
            className="h-24"
            {...register("personalInfo.summary")}
          />
          {errors.personalInfo?.summary && (
            <p className="text-sm text-red-500">{errors.personalInfo.summary.message}</p>
          )}
        </div>

        <div className="form-row mt-4">
          <div className="space-y-2">
            <Label htmlFor="linkedin">LinkedIn (opcional)</Label>
            <Input
              id="linkedin"
              placeholder="https://linkedin.com/in/seunome"
              {...register("personalInfo.linkedin")}
            />
            {errors.personalInfo?.linkedin && (
              <p className="text-sm text-red-500">{errors.personalInfo.linkedin.message}</p>
            )}
          </div>
          <div className="space-y-2">
            <Label htmlFor="github">GitHub (opcional)</Label>
            <Input
              id="github"
              placeholder="https://github.com/seunome"
              {...register("personalInfo.github")}
            />
            {errors.personalInfo?.github && (
              <p className="text-sm text-red-500">{errors.personalInfo.github.message}</p>
            )}
          </div>
          <div className="space-y-2">
            <Label htmlFor="website">Site Pessoal (opcional)</Label>
            <Input
              id="website"
              placeholder="https://seusite.com.br"
              {...register("personalInfo.website")}
            />
            {errors.personalInfo?.website && (
              <p className="text-sm text-red-500">{errors.personalInfo.website.message}</p>
            )}
          </div>
        </div>
      </div>

      {/* Education Section */}
      <div className="resume-form-section">
        <div className="flex justify-between items-center mb-4">
          <h3>Educação</h3>
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={() =>
              appendEducation({
                id: crypto.randomUUID(),
                institution: "",
                degree: "",
                fieldOfStudy: "",
                startDate: "",
                endDate: "",
                current: false,
                description: "",
              })
            }
            className="flex items-center gap-1"
          >
            <Plus className="h-4 w-4" /> Adicionar Educação
          </Button>
        </div>

        {educationFields.map((field, index) => (
          <Card key={field.id} className="mb-6 relative">
            <CardContent className="pt-6">
              <div className="form-row">
                <div className="space-y-2">
                  <Label htmlFor={`education.${index}.institution`}>Instituição</Label>
                  <Input
                    id={`education.${index}.institution`}
                    placeholder="Universidade Federal de São Paulo"
                    {...register(`education.${index}.institution`)}
                  />
                  {errors.education?.[index]?.institution && (
                    <p className="text-sm text-red-500">{errors.education[index]?.institution?.message}</p>
                  )}
                </div>
                <div className="space-y-2">
                  <Label htmlFor={`education.${index}.degree`}>Grau Acadêmico</Label>
                  <Input
                    id={`education.${index}.degree`}
                    placeholder="Bacharelado"
                    {...register(`education.${index}.degree`)}
                  />
                  {errors.education?.[index]?.degree && (
                    <p className="text-sm text-red-500">{errors.education[index]?.degree?.message}</p>
                  )}
                </div>
              </div>

              <div className="form-row">
                <div className="space-y-2">
                  <Label htmlFor={`education.${index}.fieldOfStudy`}>Área de Estudo</Label>
                  <Input
                    id={`education.${index}.fieldOfStudy`}
                    placeholder="Ciência da Computação"
                    {...register(`education.${index}.fieldOfStudy`)}
                  />
                  {errors.education?.[index]?.fieldOfStudy && (
                    <p className="text-sm text-red-500">{errors.education[index]?.fieldOfStudy?.message}</p>
                  )}
                </div>
              </div>

              <div className="form-row">
                <div className="space-y-2">
                  <Label htmlFor={`education.${index}.startDate`}>Data de Início</Label>
                  <Input
                    id={`education.${index}.startDate`}
                    placeholder="ex: 2018"
                    {...register(`education.${index}.startDate`)}
                  />
                  {errors.education?.[index]?.startDate && (
                    <p className="text-sm text-red-500">{errors.education[index]?.startDate?.message}</p>
                  )}
                </div>
                <div className="space-y-2">
                  <div className="flex justify-between">
                    <Label htmlFor={`education.${index}.endDate`}>Data de Término</Label>
                    <div className="flex items-center space-x-2">
                      <Switch
                        id={`education.${index}.current`}
                        {...register(`education.${index}.current`)}
                      />
                      <Label htmlFor={`education.${index}.current`}>Atual</Label>
                    </div>
                  </div>
                  <Input
                    id={`education.${index}.endDate`}
                    placeholder="ex: 2022"
                    disabled={watch(`education.${index}.current`)}
                    {...register(`education.${index}.endDate`)}
                  />
                </div>
              </div>

              <div className="space-y-2 mt-4">
                <Label htmlFor={`education.${index}.description`}>Descrição (opcional)</Label>
                <Textarea
                  id={`education.${index}.description`}
                  placeholder="Adicione detalhes adicionais sobre sua formação"
                  className="h-20"
                  {...register(`education.${index}.description`)}
                />
              </div>

              {educationFields.length > 1 && (
                <Button
                  type="button"
                  variant="destructive"
                  size="sm"
                  className="mt-4"
                  onClick={() => removeEducation(index)}
                >
                  <Trash2 className="h-4 w-4 mr-2" /> Remover
                </Button>
              )}
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Experience Section */}
      <div className="resume-form-section">
        <div className="flex justify-between items-center mb-4">
          <h3>Experiência Profissional</h3>
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={() =>
              appendExperience({
                id: crypto.randomUUID(),
                company: "",
                position: "",
                location: "",
                startDate: "",
                endDate: "",
                current: false,
                description: "",
              })
            }
            className="flex items-center gap-1"
          >
            <Plus className="h-4 w-4" /> Adicionar Experiência
          </Button>
        </div>

        {experienceFields.map((field, index) => (
          <Card key={field.id} className="mb-6">
            <CardContent className="pt-6">
              <div className="form-row">
                <div className="space-y-2">
                  <Label htmlFor={`experience.${index}.company`}>Empresa</Label>
                  <Input
                    id={`experience.${index}.company`}
                    placeholder="Empresa Exemplo Ltda."
                    {...register(`experience.${index}.company`)}
                  />
                  {errors.experience?.[index]?.company && (
                    <p className="text-sm text-red-500">{errors.experience[index]?.company?.message}</p>
                  )}
                </div>
                <div className="space-y-2">
                  <Label htmlFor={`experience.${index}.position`}>Cargo</Label>
                  <Input
                    id={`experience.${index}.position`}
                    placeholder="Desenvolvedor Sênior"
                    {...register(`experience.${index}.position`)}
                  />
                  {errors.experience?.[index]?.position && (
                    <p className="text-sm text-red-500">{errors.experience[index]?.position?.message}</p>
                  )}
                </div>
              </div>

              <div className="form-row">
                <div className="space-y-2">
                  <Label htmlFor={`experience.${index}.location`}>Localização</Label>
                  <Input
                    id={`experience.${index}.location`}
                    placeholder="São Paulo, SP"
                    {...register(`experience.${index}.location`)}
                  />
                  {errors.experience?.[index]?.location && (
                    <p className="text-sm text-red-500">{errors.experience[index]?.location?.message}</p>
                  )}
                </div>
              </div>

              <div className="form-row">
                <div className="space-y-2">
                  <Label htmlFor={`experience.${index}.startDate`}>Data de Início</Label>
                  <Input
                    id={`experience.${index}.startDate`}
                    placeholder="ex: Jan 2020"
                    {...register(`experience.${index}.startDate`)}
                  />
                  {errors.experience?.[index]?.startDate && (
                    <p className="text-sm text-red-500">{errors.experience[index]?.startDate?.message}</p>
                  )}
                </div>
                <div className="space-y-2">
                  <div className="flex justify-between">
                    <Label htmlFor={`experience.${index}.endDate`}>Data de Término</Label>
                    <div className="flex items-center space-x-2">
                      <Switch
                        id={`experience.${index}.current`}
                        {...register(`experience.${index}.current`)}
                      />
                      <Label htmlFor={`experience.${index}.current`}>Atual</Label>
                    </div>
                  </div>
                  <Input
                    id={`experience.${index}.endDate`}
                    placeholder="ex: Presente"
                    disabled={watch(`experience.${index}.current`)}
                    {...register(`experience.${index}.endDate`)}
                  />
                </div>
              </div>

              <div className="space-y-2 mt-4">
                <Label htmlFor={`experience.${index}.description`}>Descrição</Label>
                <Textarea
                  id={`experience.${index}.description`}
                  placeholder="Descreva suas responsabilidades e conquistas"
                  className="h-24"
                  {...register(`experience.${index}.description`)}
                />
                {errors.experience?.[index]?.description && (
                  <p className="text-sm text-red-500">{errors.experience[index]?.description?.message}</p>
                )}
              </div>

              {experienceFields.length > 1 && (
                <Button
                  type="button"
                  variant="destructive"
                  size="sm"
                  className="mt-4"
                  onClick={() => removeExperience(index)}
                >
                  <Trash2 className="h-4 w-4 mr-2" /> Remover
                </Button>
              )}
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Skills Section */}
      <div className="resume-form-section">
        <h3>Habilidades</h3>
        <div className="space-y-4">
          <div className="flex space-x-2">
            <Input
              placeholder="Adicionar uma habilidade"
              value={skillInput}
              onChange={(e) => setSkillInput(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  e.preventDefault();
                  handleAddSkill();
                }
              }}
            />
            <Button
              type="button"
              onClick={handleAddSkill}
              variant="secondary"
            >
              Adicionar
            </Button>
          </div>

          {errors.skills && (
            <p className="text-sm text-red-500">{errors.skills.message}</p>
          )}

          <div className="flex flex-wrap gap-2 mt-2">
            {skills.map((skill, index) => (
              <div
                key={index}
                className="bg-primary/10 text-primary px-3 py-1 rounded-full flex items-center"
              >
                <span>{skill}</span>
                <button
                  type="button"
                  className="ml-2 text-primary hover:text-primary/70"
                  onClick={() => handleRemoveSkill(skill)}
                >
                  &times;
                </button>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Languages Section */}
      <div className="resume-form-section">
        <div className="flex justify-between items-center mb-4">
          <h3>Idiomas</h3>
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={() =>
              appendLanguage({
                language: "",
                proficiency: "Intermediário",
              })
            }
            className="flex items-center gap-1"
          >
            <Plus className="h-4 w-4" /> Adicionar Idioma
          </Button>
        </div>

        {languageFields.map((field, index) => (
          <div key={field.id} className="flex space-x-4 items-end mb-4">
            <div className="flex-1 space-y-2">
              <Label htmlFor={`languages.${index}.language`}>Idioma</Label>
              <Input
                id={`languages.${index}.language`}
                placeholder="Português"
                {...register(`languages.${index}.language`)}
              />
              {errors.languages?.[index]?.language && (
                <p className="text-sm text-red-500">{errors.languages[index]?.language?.message}</p>
              )}
            </div>
            <div className="flex-1 space-y-2">
              <Label htmlFor={`languages.${index}.proficiency`}>Nível</Label>
              <Select
                onValueChange={(value) => setValue(`languages.${index}.proficiency`, value as any)}
                defaultValue={field.proficiency}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Selecione o nível" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Iniciante">Iniciante</SelectItem>
                  <SelectItem value="Intermediário">Intermediário</SelectItem>
                  <SelectItem value="Avançado">Avançado</SelectItem>
                  <SelectItem value="Fluente">Fluente</SelectItem>
                  <SelectItem value="Nativo">Nativo</SelectItem>
                </SelectContent>
              </Select>
            </div>
            {languageFields.length > 1 && (
              <Button
                type="button"
                variant="ghost"
                size="icon"
                onClick={() => removeLanguage(index)}
                className="mb-2"
              >
                <Trash2 className="h-4 w-4" />
              </Button>
            )}
          </div>
        ))}
      </div>

      <div className="flex justify-end space-x-4 pb-10">
        <Button type="submit" size="lg" disabled={isLoading}>
          {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
          {isLoading ? "Criando Currículo..." : "Criar Currículo"}
        </Button>
      </div>
    </form>
  );
};

export default ResumeForm;
