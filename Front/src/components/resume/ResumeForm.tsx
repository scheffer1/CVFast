
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
  CreateLanguageForCurriculumData,
  CreateContactForCurriculumData,
  CreateAddressForCurriculumData
} from "@/services/completeCurriculumService";
import curriculumService from "@/services/curriculumService";
import experienceService from "@/services/experienceService";
import educationService from "@/services/educationService";
import skillService from "@/services/skillService";
import languageService from "@/services/languageService";
import contactService from "@/services/contactService";
import addressService from "@/services/addressService";
import authService from "@/services/authService";
import { formatDateForBackend } from "@/utils/dateUtils";
import { FormDatePicker } from "@/components/ui/form-date-picker";
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
  proficiency: z.enum(["Beginner", "Intermediate", "Advanced", "Fluent", "Native"]),
});

const formSchema = z.object({
  personalInfo: personalInfoSchema,
  education: z.array(educationSchema),
  experience: z.array(experienceSchema),
  skills: z.array(z.string()).min(1, "Pelo menos uma habilidade é obrigatória"),
  languages: z.array(languageSchema),
});

type FormValues = z.infer<typeof formSchema>;

interface ResumeFormProps {
  mode?: 'create' | 'edit';
  curriculumId?: string;
  initialData?: any;
}

const ResumeForm = ({ mode = 'create', curriculumId, initialData }: ResumeFormProps) => {
  const { toast } = useToast();
  const navigate = useNavigate();
  const [skillInput, setSkillInput] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  // Função para preparar dados iniciais
  const prepareInitialData = () => {
    if (mode === 'edit' && initialData) {
      // Extrair contatos por tipo
      const emailContact = initialData.contacts?.find((c: any) => c.type === 'Email')?.value || '';
      const phoneContact = initialData.contacts?.find((c: any) => c.type === 'Phone')?.value || '';
      const linkedinContact = initialData.contacts?.find((c: any) => c.type === 'LinkedIn')?.value || '';
      const githubContact = initialData.contacts?.find((c: any) => c.type === 'GitHub')?.value || '';
      const websiteContact = initialData.contacts?.find((c: any) => c.type === 'Website')?.value || '';

      // Extrair endereço principal
      const primaryAddress = initialData.addresses?.[0];
      const addressString = primaryAddress
        ? `${primaryAddress.street}, ${primaryAddress.number}${primaryAddress.complement ? ', ' + primaryAddress.complement : ''}, ${primaryAddress.neighborhood}, ${primaryAddress.city}, ${primaryAddress.state}`
        : '';

      return {
        personalInfo: {
          fullName: initialData.title || '',
          email: emailContact,
          phone: phoneContact,
          address: addressString,
          title: initialData.title || '',
          summary: initialData.summary || '',
          linkedin: linkedinContact,
          github: githubContact,
          website: websiteContact,
        },
        education: initialData.educations?.map((edu: any) => ({
          id: edu.id,
          institution: edu.institution,
          degree: edu.degree,
          fieldOfStudy: edu.fieldOfStudy,
          startDate: edu.startDate?.split('T')[0] || '',
          current: !edu.endDate,
          endDate: edu.endDate?.split('T')[0] || '',
          description: edu.description || '',
        })) || [{
          id: crypto.randomUUID(),
          institution: "",
          degree: "",
          fieldOfStudy: "",
          startDate: "",
          endDate: "",
          current: false,
          description: "",
        }],
        experience: initialData.experiences?.map((exp: any) => ({
          id: exp.id,
          company: exp.companyName,
          position: exp.role,
          location: exp.location || '',
          startDate: exp.startDate?.split('T')[0] || '',
          current: !exp.endDate,
          endDate: exp.endDate?.split('T')[0] || '',
          description: exp.description || '',
        })) || [{
          id: crypto.randomUUID(),
          company: "",
          position: "",
          location: "",
          startDate: "",
          endDate: "",
          current: false,
          description: "",
        }],
        skills: initialData.skills?.map((skill: any) => skill.techName) || [],
        languages: initialData.languages?.map((lang: any) => ({
          language: lang.languageName,
          proficiency: lang.proficiency,
        })) || [{
          language: "",
          proficiency: "Intermediate",
        }],
      };
    }

    // Valores padrão para criação
    return {
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
          proficiency: "Intermediate",
        },
      ],
    };
  };

  const {
    register,
    control,
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: prepareInitialData(),
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
        description: "Você precisa estar logado para salvar um currículo",
      });
      navigate("/login");
      setIsLoading(false);
      return;
    }

    try {
      if (mode === 'edit' && curriculumId) {
        await handleUpdateCurriculum(data);
      } else {
        await handleCreateCurriculum(data);
      }
    } catch (error: any) {
      toast({
        variant: "destructive",
        title: mode === 'edit' ? "Erro ao atualizar currículo" : "Erro ao criar currículo",
        description: error.message || "Não foi possível salvar o currículo. Tente novamente.",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleCreateCurriculum = async (data: FormValues) => {
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

    // Preparar dados dos idiomas
    const languages: CreateLanguageForCurriculumData[] = data.languages
      .filter(lang => lang.language.trim())
      .map(lang => ({
        languageName: lang.language,
        proficiency: lang.proficiency as 'Beginner' | 'Intermediate' | 'Advanced' | 'Fluent' | 'Native'
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
      if (personalInfo.address && personalInfo.address.trim()) {
        // Se o endereço contém vírgulas, tentar dividir; senão usar o endereço completo como street
        const addressParts = personalInfo.address.split(',').map(part => part.trim()).filter(part => part.length > 0);

        addresses.push({
          street: addressParts.length > 1 ? (addressParts[0] || personalInfo.address.trim()) : personalInfo.address.trim(),
          number: 'S/N',
          neighborhood: addressParts[1] || 'Não informado',
          city: addressParts[2] || 'Não informado',
          state: addressParts[3] || 'Não informado',
          country: 'Brasil',
          type: 'Current'
        });
      }

      // Criar currículo completo
      const curriculumData: CreateCompleteCurriculumData = {
        title: data.personalInfo.fullName || "Meu Currículo",
        summary: data.personalInfo.summary,
        status: 'Active',
        experiences,
        educations,
        skills,
        languages,
        contacts,
        addresses
      };

      const curriculum = await completeCurriculumService.create(curriculumData);

      toast({
        title: "Sucesso",
        description: "Seu currículo foi criado com sucesso",
      });

      navigate(`/resume/${curriculum.id}`);
  };

  const handleUpdateCurriculum = async (data: FormValues) => {
    try {
      // 1. Atualizar dados básicos do currículo
      await curriculumService.update(curriculumId!, {
        title: data.personalInfo.fullName || "Meu Currículo",
        summary: data.personalInfo.summary,
        status: 'Active'
      });

      // 2. Atualizar experiências
      await updateExperiences(data.experience);

      // 3. Atualizar educações
      await updateEducations(data.education);

      // 4. Atualizar habilidades
      await updateSkills(data.skills);

      // 5. Atualizar idiomas
      await updateLanguages(data.languages);

      // 6. Atualizar contatos
      await updateContacts(data.personalInfo);

      // 7. Atualizar endereços
      await updateAddresses(data.personalInfo.address);

      toast({
        title: "Sucesso",
        description: "Seu currículo foi atualizado com sucesso",
      });

      navigate(`/resume/${curriculumId}`);
    } catch (error: any) {
      console.error('Erro ao atualizar currículo:', error);
      throw error; // Re-throw para ser capturado pelo onSubmit
    }
  };

  // Funções auxiliares para atualização completa
  const updateExperiences = async (newExperiences: FormValues['experience']) => {
    const originalExperiences = initialData?.experiences || [];

    for (const exp of newExperiences) {
      if (!exp.company || !exp.position) continue; // Pular experiências vazias

      const experienceData = {
        curriculumId: curriculumId!,
        companyName: exp.company,
        role: exp.position,
        description: exp.description || '',
        startDate: formatDateForBackend(exp.startDate)!,
        endDate: exp.current ? null : formatDateForBackend(exp.endDate || ''),
        location: exp.location || ''
      };

      // Se tem ID e é um GUID válido (não gerado pelo crypto.randomUUID), atualizar
      if (exp.id && originalExperiences.some((orig: any) => orig.id === exp.id)) {
        await experienceService.update(exp.id, experienceData);
      } else {
        // Criar nova experiência
        await experienceService.create(experienceData);
      }
    }

    // Deletar experiências removidas
    const newExperienceIds = newExperiences.filter(exp => exp.id && originalExperiences.some((orig: any) => orig.id === exp.id)).map(exp => exp.id);
    const experiencesToDelete = originalExperiences.filter((orig: any) => !newExperienceIds.includes(orig.id));

    for (const expToDelete of experiencesToDelete) {
      await experienceService.delete(expToDelete.id);
    }
  };

  const updateEducations = async (newEducations: FormValues['education']) => {
    const originalEducations = initialData?.educations || [];

    for (const edu of newEducations) {
      if (!edu.institution || !edu.degree) continue; // Pular educações vazias

      const educationData = {
        curriculumId: curriculumId!,
        institution: edu.institution,
        degree: edu.degree,
        fieldOfStudy: edu.fieldOfStudy,
        startDate: formatDateForBackend(edu.startDate)!,
        endDate: edu.current ? null : formatDateForBackend(edu.endDate || ''),
        description: edu.description || ''
      };

      // Se tem ID e é um GUID válido, atualizar
      if (edu.id && originalEducations.some((orig: any) => orig.id === edu.id)) {
        await educationService.update(edu.id, educationData);
      } else {
        // Criar nova educação
        await educationService.create(educationData);
      }
    }

    // Deletar educações removidas
    const newEducationIds = newEducations.filter(edu => edu.id && originalEducations.some((orig: any) => orig.id === edu.id)).map(edu => edu.id);
    const educationsToDelete = originalEducations.filter((orig: any) => !newEducationIds.includes(orig.id));

    for (const eduToDelete of educationsToDelete) {
      await educationService.delete(eduToDelete.id);
    }
  };

  const updateSkills = async (newSkills: string[]) => {
    const originalSkills = initialData?.skills || [];

    // Criar novas skills
    for (const skillName of newSkills) {
      if (!skillName.trim()) continue;

      // Verificar se a skill já existe
      const existingSkill = originalSkills.find((orig: any) => orig.techName === skillName);
      if (!existingSkill) {
        await skillService.create({
          curriculumId: curriculumId!,
          techName: skillName,
          proficiency: 'Intermediate' as const
        });
      }
    }

    // Deletar skills removidas
    const skillsToDelete = originalSkills.filter((orig: any) => !newSkills.includes(orig.techName));
    for (const skillToDelete of skillsToDelete) {
      await skillService.delete(skillToDelete.id);
    }
  };

  const updateLanguages = async (newLanguages: FormValues['languages']) => {
    const originalLanguages = initialData?.languages || [];

    for (const lang of newLanguages) {
      if (!lang.language.trim()) continue;

      // Verificar se o idioma já existe
      const existingLanguage = originalLanguages.find((orig: any) => orig.languageName === lang.language);

      if (existingLanguage) {
        // Atualizar se o nível mudou
        if (existingLanguage.proficiency !== lang.proficiency) {
          await languageService.update(existingLanguage.id, {
            curriculumId: curriculumId!,
            languageName: lang.language,
            proficiency: lang.proficiency as 'Beginner' | 'Intermediate' | 'Advanced' | 'Fluent' | 'Native'
          });
        }
      } else {
        // Criar novo idioma
        await languageService.create({
          curriculumId: curriculumId!,
          languageName: lang.language,
          proficiency: lang.proficiency as 'Beginner' | 'Intermediate' | 'Advanced' | 'Fluent' | 'Native'
        });
      }
    }

    // Deletar idiomas removidos
    const newLanguageNames = newLanguages.map(lang => lang.language);
    const languagesToDelete = originalLanguages.filter((orig: any) => !newLanguageNames.includes(orig.languageName));
    for (const langToDelete of languagesToDelete) {
      await languageService.delete(langToDelete.id);
    }
  };

  const updateContacts = async (personalInfo: FormValues['personalInfo']) => {
    const originalContacts = initialData?.contacts || [];

    const contactsToUpdate = [
      { type: 'Email', value: personalInfo.email, isPrimary: true },
      { type: 'Phone', value: personalInfo.phone, isPrimary: false },
      { type: 'LinkedIn', value: personalInfo.linkedin, isPrimary: false },
      { type: 'GitHub', value: personalInfo.github, isPrimary: false },
      { type: 'Website', value: personalInfo.website, isPrimary: false }
    ].filter(contact => contact.value && contact.value.trim());

    for (const contact of contactsToUpdate) {
      const existingContact = originalContacts.find((orig: any) => orig.type === contact.type);

      if (existingContact) {
        // Atualizar se o valor mudou
        if (existingContact.value !== contact.value) {
          await contactService.update(existingContact.id, {
            curriculumId: curriculumId!,
            type: contact.type as 'Email' | 'Phone' | 'LinkedIn' | 'GitHub' | 'Website',
            value: contact.value,
            isPrimary: contact.isPrimary
          });
        }
      } else {
        // Criar novo contato
        await contactService.create({
          curriculumId: curriculumId!,
          type: contact.type as 'Email' | 'Phone' | 'LinkedIn' | 'GitHub' | 'Website',
          value: contact.value,
          isPrimary: contact.isPrimary
        });
      }
    }

    // Deletar contatos removidos (que não têm valor nos novos dados)
    const newContactTypes = contactsToUpdate.map(c => c.type);
    const contactsToDelete = originalContacts.filter((orig: any) => !newContactTypes.includes(orig.type));
    for (const contactToDelete of contactsToDelete) {
      await contactService.delete(contactToDelete.id);
    }
  };

  const updateAddresses = async (addressString: string) => {
    const originalAddresses = initialData?.addresses || [];

    if (addressString && addressString.trim()) {
      const addressParts = addressString.split(',').map(part => part.trim()).filter(part => part.length > 0);

      const addressData = {
        curriculumId: curriculumId!,
        street: addressParts.length > 1 ? addressParts[0] : addressString.trim(),
        number: 'S/N',
        complement: '',
        neighborhood: addressParts[1] || 'Não informado',
        city: addressParts[2] || 'Não informado',
        state: addressParts[3] || 'Não informado',
        country: 'Brasil',
        zipCode: '',
        type: 'Current' as const
      };

      if (originalAddresses.length > 0) {
        // Atualizar endereço existente
        await addressService.update(originalAddresses[0].id, addressData);
      } else {
        // Criar novo endereço
        await addressService.create(addressData);
      }
    } else {
      // Se não há endereço, deletar todos os endereços existentes
      for (const addressToDelete of originalAddresses) {
        await addressService.delete(addressToDelete.id);
      }
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
                <FormDatePicker
                  control={control}
                  name={`education.${index}.startDate`}
                  label="Data de Início"
                  placeholder="Selecione a data de início"
                  error={errors.education?.[index]?.startDate?.message}
                />
                <div className="space-y-2">
                  <div className="flex justify-between">
                    <Label htmlFor={`education.${index}.endDate`}>Data de Término</Label>
                    <div className="flex items-center space-x-2">
                      <Switch
                        id={`education.${index}.current`}
                        checked={watch(`education.${index}.current`)}
                        onCheckedChange={(checked) => {
                          setValue(`education.${index}.current`, checked);
                          if (checked) {
                            setValue(`education.${index}.endDate`, '');
                          }
                        }}
                      />
                      <Label htmlFor={`education.${index}.current`}>Atual</Label>
                    </div>
                  </div>
                  <FormDatePicker
                    control={control}
                    name={`education.${index}.endDate`}
                    placeholder="Selecione a data de término"
                    disabled={watch(`education.${index}.current`)}
                    error={errors.education?.[index]?.endDate?.message}
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
                <FormDatePicker
                  control={control}
                  name={`experience.${index}.startDate`}
                  label="Data de Início"
                  placeholder="Selecione a data de início"
                  error={errors.experience?.[index]?.startDate?.message}
                />
                <div className="space-y-2">
                  <div className="flex justify-between">
                    <Label htmlFor={`experience.${index}.endDate`}>Data de Término</Label>
                    <div className="flex items-center space-x-2">
                      <Switch
                        id={`experience.${index}.current`}
                        checked={watch(`experience.${index}.current`)}
                        onCheckedChange={(checked) => {
                          setValue(`experience.${index}.current`, checked);
                          if (checked) {
                            setValue(`experience.${index}.endDate`, '');
                          }
                        }}
                      />
                      <Label htmlFor={`experience.${index}.current`}>Atual</Label>
                    </div>
                  </div>
                  <FormDatePicker
                    control={control}
                    name={`experience.${index}.endDate`}
                    placeholder="Selecione a data de término"
                    disabled={watch(`experience.${index}.current`)}
                    error={errors.experience?.[index]?.endDate?.message}
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
                proficiency: "Intermediate",
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
                  <SelectItem value="Beginner">Iniciante</SelectItem>
                  <SelectItem value="Intermediate">Intermediário</SelectItem>
                  <SelectItem value="Advanced">Avançado</SelectItem>
                  <SelectItem value="Fluent">Fluente</SelectItem>
                  <SelectItem value="Native">Nativo</SelectItem>
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
          {isLoading
            ? (mode === 'edit' ? "Atualizando Currículo..." : "Criando Currículo...")
            : (mode === 'edit' ? "Atualizar Currículo" : "Criar Currículo")
          }
        </Button>
      </div>
    </form>
  );
};

export default ResumeForm;
