
import React, { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import Navbar from "@/components/shared/Navbar";
import { getUser, getResumesByUserId, deleteResume } from "@/utils/localStorage";
import { Resume } from "@/types";
import { FileText, Plus, Trash2, Link as LinkIcon } from "lucide-react";
import { useToast } from "@/components/ui/use-toast";

const Dashboard = () => {
  const { toast } = useToast();
  const navigate = useNavigate();
  const [resumes, setResumes] = useState<Resume[]>([]);
  const [user, setUser] = useState(getUser());

  useEffect(() => {
    if (!user) {
      navigate("/login");
      return;
    }

    const userResumes = getResumesByUserId(user.id);
    setResumes(userResumes);
  }, [user, navigate]);

  const handleDeleteResume = (id: string) => {
    if (window.confirm("Are you sure you want to delete this resume?")) {
      deleteResume(id);
      setResumes(resumes.filter(resume => resume.id !== id));
      toast({
        title: "Resume deleted",
        description: "Your resume has been permanently deleted",
      });
    }
  };

  const copyLink = (link: string) => {
    navigator.clipboard.writeText(link);
    toast({
      title: "Link copied",
      description: "The resume link has been copied to your clipboard",
    });
  };

  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      <Navbar />
      <div className="container mx-auto px-4 py-8 flex-1">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold">My Resumes</h1>
          <Link to="/create-resume">
            <Button className="flex items-center gap-2">
              <Plus className="h-4 w-4" /> Create New Resume
            </Button>
          </Link>
        </div>

        {resumes.length === 0 ? (
          <Card className="bg-white border-dashed border-2 border-gray-300">
            <CardContent className="flex flex-col items-center justify-center py-12">
              <FileText className="h-16 w-16 text-gray-400 mb-4" />
              <h3 className="text-xl font-semibold text-gray-700 mb-2">No resumes yet</h3>
              <p className="text-gray-500 text-center max-w-md mb-6">
                Create your first resume to share with potential employers
              </p>
              <Link to="/create-resume">
                <Button>Create Resume</Button>
              </Link>
            </CardContent>
          </Card>
        ) : (
          <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
            {resumes.map((resume) => (
              <Card key={resume.id} className="bg-white">
                <CardHeader>
                  <CardTitle className="flex justify-between items-start">
                    <span className="truncate">{resume.personalInfo.fullName}</span>
                    <Button
                      variant="ghost"
                      size="icon"
                      className="text-red-500 hover:text-red-700 hover:bg-red-50"
                      onClick={() => handleDeleteResume(resume.id)}
                    >
                      <Trash2 className="h-5 w-5" />
                    </Button>
                  </CardTitle>
                  <CardDescription>
                    {resume.personalInfo.title}
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="space-y-3">
                    <div>
                      <h4 className="text-sm font-medium text-gray-500">Skills</h4>
                      <div className="flex flex-wrap gap-1 mt-1">
                        {resume.skills.slice(0, 3).map((skill, index) => (
                          <span key={index} className="bg-primary/10 text-primary text-xs px-2 py-1 rounded">
                            {skill}
                          </span>
                        ))}
                        {resume.skills.length > 3 && (
                          <span className="bg-secondary text-secondary-foreground text-xs px-2 py-1 rounded">
                            +{resume.skills.length - 3} more
                          </span>
                        )}
                      </div>
                    </div>
                    <div>
                      <h4 className="text-sm font-medium text-gray-500">Experience</h4>
                      <p className="text-sm truncate">
                        {resume.experience.length > 0
                          ? `${resume.experience[0].position} at ${resume.experience[0].company}`
                          : "No experience listed"}
                      </p>
                    </div>
                    <div>
                      <h4 className="text-sm font-medium text-gray-500">Last Updated</h4>
                      <p className="text-sm">
                        {new Date(resume.updatedAt).toLocaleDateString()}
                      </p>
                    </div>
                  </div>
                </CardContent>
                <CardFooter className="flex flex-col space-y-3">
                  <Link to={`/resume/${resume.id}`} className="w-full">
                    <Button variant="default" className="w-full">
                      View Resume
                    </Button>
                  </Link>
                  <Button
                    variant="outline"
                    className="w-full flex items-center gap-2"
                    onClick={() => copyLink(resume.shareableLink)}
                  >
                    <LinkIcon className="h-4 w-4" /> Copy Shareable Link
                  </Button>
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
