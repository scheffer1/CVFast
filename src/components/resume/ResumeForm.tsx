
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
import { getUser, saveResume } from "@/utils/localStorage";
import { Plus, Trash2 } from "lucide-react";

const personalInfoSchema = z.object({
  fullName: z.string().min(2, "Full name is required"),
  email: z.string().email("Valid email is required"),
  phone: z.string().min(6, "Phone number is required"),
  address: z.string().min(5, "Address is required"),
  title: z.string().min(2, "Professional title is required"),
  summary: z.string().min(10, "Professional summary is required"),
  linkedin: z.string().url("Must be a valid URL").or(z.string().length(0)).optional(),
  github: z.string().url("Must be a valid URL").or(z.string().length(0)).optional(),
  website: z.string().url("Must be a valid URL").or(z.string().length(0)).optional(),
});

const educationSchema = z.object({
  id: z.string(),
  institution: z.string().min(2, "Institution name is required"),
  degree: z.string().min(2, "Degree is required"),
  fieldOfStudy: z.string().min(2, "Field of study is required"),
  startDate: z.string().min(4, "Start date is required"),
  endDate: z.string().optional(),
  current: z.boolean(),
  description: z.string().optional(),
});

const experienceSchema = z.object({
  id: z.string(),
  company: z.string().min(2, "Company name is required"),
  position: z.string().min(2, "Position is required"),
  location: z.string().min(2, "Location is required"),
  startDate: z.string().min(4, "Start date is required"),
  endDate: z.string().optional(),
  current: z.boolean(),
  description: z.string().min(10, "Job description is required"),
});

const languageSchema = z.object({
  language: z.string().min(2, "Language name is required"),
  proficiency: z.enum(["Beginner", "Intermediate", "Advanced", "Fluent", "Native"]),
});

const formSchema = z.object({
  personalInfo: personalInfoSchema,
  education: z.array(educationSchema),
  experience: z.array(experienceSchema),
  skills: z.array(z.string()).min(1, "At least one skill is required"),
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
          proficiency: "Intermediate",
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

  const onSubmit = (data: FormValues) => {
    setIsLoading(true);
    const user = getUser();

    if (!user) {
      toast({
        variant: "destructive",
        title: "Error",
        description: "You must be logged in to create a resume",
      });
      navigate("/login");
      return;
    }

    // Create shareable link
    const uniqueId = crypto.randomUUID();
    const shareableLink = `${window.location.origin}/resume/${uniqueId}`;

    const newResume: Resume = {
      id: uniqueId,
      userId: user.id,
      personalInfo: data.personalInfo as PersonalInfo,
      education: data.education as Education[],
      experience: data.experience as Experience[],
      skills: data.skills,
      languages: data.languages as Language[],
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      shareableLink,
    };

    // Save resume to local storage
    setTimeout(() => {
      saveResume(newResume);
      toast({
        title: "Success",
        description: "Your resume has been created",
      });
      navigate(`/resume/${uniqueId}`);
      setIsLoading(false);
    }, 1000);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-8">
      {/* Personal Info Section */}
      <div className="resume-form-section">
        <h3>Personal Information</h3>
        <div className="form-row">
          <div className="space-y-2">
            <Label htmlFor="fullName">Full Name</Label>
            <Input
              id="fullName"
              placeholder="John Doe"
              {...register("personalInfo.fullName")}
            />
            {errors.personalInfo?.fullName && (
              <p className="text-sm text-red-500">{errors.personalInfo.fullName.message}</p>
            )}
          </div>
          <div className="space-y-2">
            <Label htmlFor="title">Professional Title</Label>
            <Input
              id="title"
              placeholder="Software Engineer"
              {...register("personalInfo.title")}
            />
            {errors.personalInfo?.title && (
              <p className="text-sm text-red-500">{errors.personalInfo.title.message}</p>
            )}
          </div>
        </div>

        <div className="form-row">
          <div className="space-y-2">
            <Label htmlFor="email">Email</Label>
            <Input
              id="email"
              placeholder="john.doe@example.com"
              {...register("personalInfo.email")}
            />
            {errors.personalInfo?.email && (
              <p className="text-sm text-red-500">{errors.personalInfo.email.message}</p>
            )}
          </div>
          <div className="space-y-2">
            <Label htmlFor="phone">Phone</Label>
            <Input
              id="phone"
              placeholder="+1 (234) 567-8901"
              {...register("personalInfo.phone")}
            />
            {errors.personalInfo?.phone && (
              <p className="text-sm text-red-500">{errors.personalInfo.phone.message}</p>
            )}
          </div>
        </div>

        <div className="space-y-2 mt-4">
          <Label htmlFor="address">Address</Label>
          <Input
            id="address"
            placeholder="123 Main St, City, State, Country"
            {...register("personalInfo.address")}
          />
          {errors.personalInfo?.address && (
            <p className="text-sm text-red-500">{errors.personalInfo.address.message}</p>
          )}
        </div>

        <div className="space-y-2 mt-4">
          <Label htmlFor="summary">Professional Summary</Label>
          <Textarea
            id="summary"
            placeholder="Briefly describe your professional background and skills"
            className="h-24"
            {...register("personalInfo.summary")}
          />
          {errors.personalInfo?.summary && (
            <p className="text-sm text-red-500">{errors.personalInfo.summary.message}</p>
          )}
        </div>

        <div className="form-row mt-4">
          <div className="space-y-2">
            <Label htmlFor="linkedin">LinkedIn (optional)</Label>
            <Input
              id="linkedin"
              placeholder="https://linkedin.com/in/johndoe"
              {...register("personalInfo.linkedin")}
            />
            {errors.personalInfo?.linkedin && (
              <p className="text-sm text-red-500">{errors.personalInfo.linkedin.message}</p>
            )}
          </div>
          <div className="space-y-2">
            <Label htmlFor="github">GitHub (optional)</Label>
            <Input
              id="github"
              placeholder="https://github.com/johndoe"
              {...register("personalInfo.github")}
            />
            {errors.personalInfo?.github && (
              <p className="text-sm text-red-500">{errors.personalInfo.github.message}</p>
            )}
          </div>
          <div className="space-y-2">
            <Label htmlFor="website">Personal Website (optional)</Label>
            <Input
              id="website"
              placeholder="https://johndoe.com"
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
          <h3>Education</h3>
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
            <Plus className="h-4 w-4" /> Add Education
          </Button>
        </div>

        {educationFields.map((field, index) => (
          <Card key={field.id} className="mb-6 relative">
            <CardContent className="pt-6">
              <div className="form-row">
                <div className="space-y-2">
                  <Label htmlFor={`education.${index}.institution`}>Institution</Label>
                  <Input
                    id={`education.${index}.institution`}
                    placeholder="University of Example"
                    {...register(`education.${index}.institution`)}
                  />
                  {errors.education?.[index]?.institution && (
                    <p className="text-sm text-red-500">{errors.education[index]?.institution?.message}</p>
                  )}
                </div>
                <div className="space-y-2">
                  <Label htmlFor={`education.${index}.degree`}>Degree</Label>
                  <Input
                    id={`education.${index}.degree`}
                    placeholder="Bachelor of Science"
                    {...register(`education.${index}.degree`)}
                  />
                  {errors.education?.[index]?.degree && (
                    <p className="text-sm text-red-500">{errors.education[index]?.degree?.message}</p>
                  )}
                </div>
              </div>

              <div className="form-row">
                <div className="space-y-2">
                  <Label htmlFor={`education.${index}.fieldOfStudy`}>Field of Study</Label>
                  <Input
                    id={`education.${index}.fieldOfStudy`}
                    placeholder="Computer Science"
                    {...register(`education.${index}.fieldOfStudy`)}
                  />
                  {errors.education?.[index]?.fieldOfStudy && (
                    <p className="text-sm text-red-500">{errors.education[index]?.fieldOfStudy?.message}</p>
                  )}
                </div>
              </div>

              <div className="form-row">
                <div className="space-y-2">
                  <Label htmlFor={`education.${index}.startDate`}>Start Date</Label>
                  <Input
                    id={`education.${index}.startDate`}
                    placeholder="e.g., 2018"
                    {...register(`education.${index}.startDate`)}
                  />
                  {errors.education?.[index]?.startDate && (
                    <p className="text-sm text-red-500">{errors.education[index]?.startDate?.message}</p>
                  )}
                </div>
                <div className="space-y-2">
                  <div className="flex justify-between">
                    <Label htmlFor={`education.${index}.endDate`}>End Date</Label>
                    <div className="flex items-center space-x-2">
                      <Switch
                        id={`education.${index}.current`}
                        {...register(`education.${index}.current`)}
                      />
                      <Label htmlFor={`education.${index}.current`}>Current</Label>
                    </div>
                  </div>
                  <Input
                    id={`education.${index}.endDate`}
                    placeholder="e.g., 2022"
                    disabled={watch(`education.${index}.current`)}
                    {...register(`education.${index}.endDate`)}
                  />
                </div>
              </div>

              <div className="space-y-2 mt-4">
                <Label htmlFor={`education.${index}.description`}>Description (optional)</Label>
                <Textarea
                  id={`education.${index}.description`}
                  placeholder="Add any additional details about your education"
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
                  <Trash2 className="h-4 w-4 mr-2" /> Remove
                </Button>
              )}
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Experience Section */}
      <div className="resume-form-section">
        <div className="flex justify-between items-center mb-4">
          <h3>Work Experience</h3>
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
            <Plus className="h-4 w-4" /> Add Experience
          </Button>
        </div>

        {experienceFields.map((field, index) => (
          <Card key={field.id} className="mb-6">
            <CardContent className="pt-6">
              <div className="form-row">
                <div className="space-y-2">
                  <Label htmlFor={`experience.${index}.company`}>Company</Label>
                  <Input
                    id={`experience.${index}.company`}
                    placeholder="Acme Inc."
                    {...register(`experience.${index}.company`)}
                  />
                  {errors.experience?.[index]?.company && (
                    <p className="text-sm text-red-500">{errors.experience[index]?.company?.message}</p>
                  )}
                </div>
                <div className="space-y-2">
                  <Label htmlFor={`experience.${index}.position`}>Position</Label>
                  <Input
                    id={`experience.${index}.position`}
                    placeholder="Senior Developer"
                    {...register(`experience.${index}.position`)}
                  />
                  {errors.experience?.[index]?.position && (
                    <p className="text-sm text-red-500">{errors.experience[index]?.position?.message}</p>
                  )}
                </div>
              </div>

              <div className="form-row">
                <div className="space-y-2">
                  <Label htmlFor={`experience.${index}.location`}>Location</Label>
                  <Input
                    id={`experience.${index}.location`}
                    placeholder="San Francisco, CA"
                    {...register(`experience.${index}.location`)}
                  />
                  {errors.experience?.[index]?.location && (
                    <p className="text-sm text-red-500">{errors.experience[index]?.location?.message}</p>
                  )}
                </div>
              </div>

              <div className="form-row">
                <div className="space-y-2">
                  <Label htmlFor={`experience.${index}.startDate`}>Start Date</Label>
                  <Input
                    id={`experience.${index}.startDate`}
                    placeholder="e.g., Jan 2020"
                    {...register(`experience.${index}.startDate`)}
                  />
                  {errors.experience?.[index]?.startDate && (
                    <p className="text-sm text-red-500">{errors.experience[index]?.startDate?.message}</p>
                  )}
                </div>
                <div className="space-y-2">
                  <div className="flex justify-between">
                    <Label htmlFor={`experience.${index}.endDate`}>End Date</Label>
                    <div className="flex items-center space-x-2">
                      <Switch
                        id={`experience.${index}.current`}
                        {...register(`experience.${index}.current`)}
                      />
                      <Label htmlFor={`experience.${index}.current`}>Current</Label>
                    </div>
                  </div>
                  <Input
                    id={`experience.${index}.endDate`}
                    placeholder="e.g., Present"
                    disabled={watch(`experience.${index}.current`)}
                    {...register(`experience.${index}.endDate`)}
                  />
                </div>
              </div>

              <div className="space-y-2 mt-4">
                <Label htmlFor={`experience.${index}.description`}>Description</Label>
                <Textarea
                  id={`experience.${index}.description`}
                  placeholder="Describe your responsibilities and achievements"
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
                  <Trash2 className="h-4 w-4 mr-2" /> Remove
                </Button>
              )}
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Skills Section */}
      <div className="resume-form-section">
        <h3>Skills</h3>
        <div className="space-y-4">
          <div className="flex space-x-2">
            <Input
              placeholder="Add a skill"
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
              Add
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
          <h3>Languages</h3>
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
            <Plus className="h-4 w-4" /> Add Language
          </Button>
        </div>

        {languageFields.map((field, index) => (
          <div key={field.id} className="flex space-x-4 items-end mb-4">
            <div className="flex-1 space-y-2">
              <Label htmlFor={`languages.${index}.language`}>Language</Label>
              <Input
                id={`languages.${index}.language`}
                placeholder="English"
                {...register(`languages.${index}.language`)}
              />
              {errors.languages?.[index]?.language && (
                <p className="text-sm text-red-500">{errors.languages[index]?.language?.message}</p>
              )}
            </div>
            <div className="flex-1 space-y-2">
              <Label htmlFor={`languages.${index}.proficiency`}>Proficiency</Label>
              <Select
                onValueChange={(value) => setValue(`languages.${index}.proficiency`, value as any)}
                defaultValue={field.proficiency}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select proficiency" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Beginner">Beginner</SelectItem>
                  <SelectItem value="Intermediate">Intermediate</SelectItem>
                  <SelectItem value="Advanced">Advanced</SelectItem>
                  <SelectItem value="Fluent">Fluent</SelectItem>
                  <SelectItem value="Native">Native</SelectItem>
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
          {isLoading ? "Creating Resume..." : "Create Resume"}
        </Button>
      </div>
    </form>
  );
};

export default ResumeForm;
